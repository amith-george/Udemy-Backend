# Automated API Test Runner for Udemy Clone Backend
# Targets port 5291 (localhost)

# Load System.Net.Http assembly (Required for Windows PowerShell)
Add-Type -AssemblyName System.Net.Http

# Enable TLS 1.2
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# Define Variables
$BaseUrl = "http://localhost:5291"
$MysqlPath = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"
$DbUser = "root"
$DbPass = "root"
$DbName = "udemy_db"

$InstructorEmail = "instructor_$(Get-Random)@example.com"
$StudentEmail = "student_$(Get-Random)@example.com"
$TempEmail = "temp_$(Get-Random)@example.com"
$Password = "SecurePassword123"

# Create dummy upload files
"dummy image content" | Out-File -FilePath "dummy_thumbnail.jpg" -Encoding ASCII -Force
"dummy zip content" | Out-File -FilePath "dummy_resource.zip" -Encoding ASCII -Force
$ThumbPath = Resolve-Path "dummy_thumbnail.jpg"
$ResourcePath = Resolve-Path "dummy_resource.zip"

# Test results array
$Results = @()

# Helper: Run SQL Query
function Execute-SqlQuery {
    param([string]$Query)
    try {
        $result = & $MysqlPath -u $DbUser "-p$DbPass" -D $DbName -e "$Query" -s -N 2>$null
        return $result
    } catch {
        Write-Warning "SQL Error: $_"
        return $null
    }
}

# Helper: Get HMAC-SHA256 signature
function Get-HmacSha256 {
    param(
        [string]$Message,
        [string]$Key
    )
    $encoding = New-Object System.Text.UTF8Encoding
    $hmac = New-Object System.Security.Cryptography.HMACSHA256
    $hmac.Key = $encoding.GetBytes($key)
    $hash = $hmac.ComputeHash($encoding.GetBytes($Message))
    return [System.BitConverter]::ToString($hash).Replace("-", "").ToLower()
}

# Helper: Generic HTTP Request
function Invoke-ApiRequest {
    param(
        [string]$Name,
        [string]$Endpoint,
        [string]$Method = "GET",
        [string]$JsonBody = $null,
        [string]$Token = $null,
        [string]$ExpectedStatus = "200"
    )

    $Url = "$BaseUrl$Endpoint"
    Write-Host "Running: $Name ($Method $Endpoint)..." -ForegroundColor Cyan
    
    $request = [System.Net.Http.HttpRequestMessage]::new()
    $request.RequestUri = [System.Uri]::new($Url)
    $request.Method = [System.Net.Http.HttpMethod]::new($Method)

    if ($Token) {
        $request.Headers.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new("Bearer", $Token)
    }

    if ($JsonBody) {
        $request.Content = [System.Net.Http.StringContent]::new($JsonBody, [System.Text.Encoding]::UTF8, "application/json")
    }

    $client = [System.Net.Http.HttpClient]::new()
    $client.Timeout = [System.TimeSpan]::FromSeconds(10)
    
    $resultObj = [PSCustomObject]@{
        Name = $Name
        Method = $Method
        Endpoint = $Endpoint
        RequestPayload = $JsonBody
        StatusCode = 0
        ResponseBody = ""
        Status = "FAIL"
    }

    try {
        $response = $client.SendAsync($request).Result
        $responseBody = $response.Content.ReadAsStringAsync().Result
        $statusCode = [int]$response.StatusCode
        
        $resultObj.StatusCode = $statusCode
        $resultObj.ResponseBody = $responseBody

        $expectedList = $ExpectedStatus -split ","
        if ($expectedList -contains $statusCode.ToString()) {
            $resultObj.Status = "PASS"
            Write-Host "  Success ($statusCode)" -ForegroundColor Green
        } else {
            Write-Host "  Failed (Expected $ExpectedStatus, Got $statusCode). Body: $responseBody" -ForegroundColor Red
        }
    } catch {
        $resultObj.ResponseBody = $_.Exception.ToString()
        Write-Host "  Error: $_" -ForegroundColor Red
    } finally {
        $client.Dispose()
    }

    $script:Results += $resultObj
    return $resultObj
}

# Helper: Multipart Request
function Invoke-ApiMultipartRequest {
    param(
        [string]$Name,
        [string]$Endpoint,
        [string]$Method = "POST",
        [hashtable]$Fields = @{},
        [hashtable]$FileFields = @{},
        [string]$Token = $null,
        [string]$ExpectedStatus = "200"
    )

    $Url = "$BaseUrl$Endpoint"
    Write-Host "Running Multipart: $Name ($Method $Endpoint)..." -ForegroundColor Cyan

    $client = [System.Net.Http.HttpClient]::new()
    $client.Timeout = [System.TimeSpan]::FromSeconds(15)
    if ($Token) {
        $client.DefaultRequestHeaders.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new("Bearer", $Token)
    }

    $boundary = "----TestBoundary" + [System.Guid]::NewGuid().ToString()
    $content = [System.Net.Http.MultipartFormDataContent]::new($boundary)

    # Add text fields
    foreach ($key in $Fields.Keys) {
        $val = $Fields[$key]
        $stringContent = [System.Net.Http.StringContent]::new($val)
        $content.Add($stringContent, $key)
    }

    # Add file fields
    $openedStreams = @()
    
    $resultObj = [PSCustomObject]@{
        Name = $Name
        Method = $Method
        Endpoint = $Endpoint
        RequestPayload = "Multipart Form Data: " + ($Fields | ConvertTo-Json -Compress)
        StatusCode = 0
        ResponseBody = ""
        Status = "FAIL"
    }

    try {
        foreach ($key in $FileFields.Keys) {
            $filePath = $FileFields[$key]
            if (Test-Path $filePath) {
                $fileStream = [System.IO.File]::OpenRead($filePath)
                $openedStreams += $fileStream
                $streamContent = [System.Net.Http.StreamContent]::new($fileStream)
                $fileName = [System.IO.Path]::GetFileName($filePath)
                $content.Add($streamContent, $key, $fileName)
            }
        }

        $response = $null
        if ($Method -eq "POST") {
            $response = $client.PostAsync($Url, $content).Result
        } else {
            # PUT
            $request = [System.Net.Http.HttpRequestMessage]::new()
            $request.RequestUri = [System.Uri]::new($Url)
            $request.Method = [System.Net.Http.HttpMethod]::new("PUT")
            $request.Content = $content
            $response = $client.SendAsync($request).Result
        }

        $responseBody = $response.Content.ReadAsStringAsync().Result
        $statusCode = [int]$response.StatusCode

        $resultObj.StatusCode = $statusCode
        $resultObj.ResponseBody = $responseBody

        $expectedList = $ExpectedStatus -split ","
        if ($expectedList -contains $statusCode.ToString()) {
            $resultObj.Status = "PASS"
            Write-Host "  Success ($statusCode)" -ForegroundColor Green
        } else {
            Write-Host "  Failed (Expected $ExpectedStatus, Got $statusCode). Body: $responseBody" -ForegroundColor Red
        }
    } catch {
        $resultObj.ResponseBody = $_.Exception.ToString()
        Write-Host "  Error: $_" -ForegroundColor Red
    } finally {
        foreach ($stream in $openedStreams) {
            $stream.Dispose()
        }
        $client.Dispose()
    }

    $script:Results += $resultObj
    return $resultObj
}

# ==========================================
# TEST SEQUENCE START
# ==========================================

Write-Host "=== STARTING UDEMY BACKEND API TEST SUITE ===" -ForegroundColor Yellow

# --- PHASE 1: AUTHENTICATION ---

# 1. Register Instructor Account
$payload = @{
    fullName = "Test Instructor"
    email = $InstructorEmail
    password = $Password
    systemRole = 1
} | ConvertTo-Json
$r = Invoke-ApiRequest "Register Instructor" "/api/Auth/register" "POST" $payload -ExpectedStatus "200"

# Query Instructor OTP from DB
Start-Sleep -Seconds 1
$InstructorOtp = Execute-SqlQuery "SELECT otp FROM users WHERE Email = '$InstructorEmail';"
Write-Host "Instructor OTP fetched: $InstructorOtp" -ForegroundColor Gray

# 2. Verify Instructor OTP
$payload = @{
    email = $InstructorEmail
    otp = $InstructorOtp
} | ConvertTo-Json
$r = Invoke-ApiRequest "Verify Instructor OTP" "/api/Auth/verify-otp" "POST" $payload -ExpectedStatus "200"
$InstructorToken = $null
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $InstructorToken = $json.token
}

# 3. Log In Instructor
$payload = @{
    email = $InstructorEmail
    password = $Password
} | ConvertTo-Json
$r = Invoke-ApiRequest "Log In Instructor" "/api/Auth/login" "POST" $payload -ExpectedStatus "200"

# 4. Register Student Account
$payload = @{
    fullName = "Test Student"
    email = $StudentEmail
    password = $Password
    systemRole = 2
} | ConvertTo-Json
$r = Invoke-ApiRequest "Register Student" "/api/Auth/register" "POST" $payload -ExpectedStatus "200"

# Query Student OTP from DB
Start-Sleep -Seconds 1
$StudentOtp = Execute-SqlQuery "SELECT otp FROM users WHERE Email = '$StudentEmail';"
Write-Host "Student OTP fetched: $StudentOtp" -ForegroundColor Gray

# 5. Verify Student OTP
$payload = @{
    email = $StudentEmail
    otp = $StudentOtp
} | ConvertTo-Json
$r = Invoke-ApiRequest "Verify Student OTP" "/api/Auth/verify-otp" "POST" $payload -ExpectedStatus "200"
$StudentToken = $null
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $StudentToken = $json.token
}

# 6. Log In Student
$payload = @{
    email = $StudentEmail
    password = $Password
} | ConvertTo-Json
$r = Invoke-ApiRequest "Log In Student" "/api/Auth/login" "POST" $payload -ExpectedStatus "200"

# 7. Resend OTP Test (Register a temp user, resend OTP)
$payload = @{
    fullName = "Temp User"
    email = $TempEmail
    password = $Password
    systemRole = 2
} | ConvertTo-Json
$r = Invoke-ApiRequest "Register Temp User (for Resend OTP test)" "/api/Auth/register" "POST" $payload -ExpectedStatus "200"

$payload = @{
    email = $TempEmail
} | ConvertTo-Json
$r = Invoke-ApiRequest "Resend OTP" "/api/Auth/resend-otp" "POST" $payload -ExpectedStatus "200"


# --- GET PROFILE DETAILS ---
$StudentId = 0
$InstructorId = 0

# 8. Get Instructor Profile
$r = Invoke-ApiRequest "Get Instructor Profile" "/api/User/profile" "GET" -Token $InstructorToken -ExpectedStatus "200"
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $InstructorId = $json.instructorDetails.instructorId
}

# 9. Get Student Profile
$r = Invoke-ApiRequest "Get Student Profile" "/api/User/profile" "GET" -Token $StudentToken -ExpectedStatus "200"
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $StudentId = $json.studentDetails.studentId
}

Write-Host "Instructor ID: $InstructorId, Student ID: $StudentId" -ForegroundColor Gray


# --- PHASE 2: CATEGORIES & COURSE ADMINISTRATION ---

# 10. Create Category
$payload = @{
    name = "Development Test $(Get-Random)"
    description = "Software development and programming courses for testing"
} | ConvertTo-Json
$r = Invoke-ApiRequest "Create Category" "/api/Categories" "POST" $payload -ExpectedStatus "201,200"
$CategoryId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $CategoryId = $json.id
}

# 11. Get Categories
$r = Invoke-ApiRequest "Get Categories" "/api/Categories" "GET" -ExpectedStatus "200"

# 12. Get Categories With Subcategories
$r = Invoke-ApiRequest "Get Categories Tree" "/api/Categories/with-subcategories" "GET" -ExpectedStatus "200"

# 13. Get Category by ID
$r = Invoke-ApiRequest "Get Category by ID" "/api/Categories/$CategoryId" "GET" -ExpectedStatus "200"

# 14. Create Subcategory
$payload = @{
    name = "C# & .NET Core Test"
    categoryId = $CategoryId
} | ConvertTo-Json
$r = Invoke-ApiRequest "Create Subcategory" "/api/Categories/subcategories" "POST" $payload -ExpectedStatus "200"
$SubcategoryId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $SubcategoryId = $json.id
}

# 15. Create Course (Multipart)
$fields = @{
    Title = "Learn C# from Scratch - AutoTest"
    Description = "Learn C# from scratch with automated verification"
    Price = "499.00"
    Status = "Premium"
    SubcategoryId = $SubcategoryId.ToString()
}
$fileFields = @{
    ThumbnailImage = $ThumbPath
}
$r = Invoke-ApiMultipartRequest "Create Course" "/api/Courses" "POST" $fields $fileFields -Token $InstructorToken -ExpectedStatus "201"
$CourseId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $CourseId = $json.courseId
}

# 16. Get Courses (Catalog)
$r = Invoke-ApiRequest "Get Courses Catalog" "/api/Courses" "GET" -ExpectedStatus "200"

# 17. Get Course Details
$r = Invoke-ApiRequest "Get Course Details" "/api/Courses/$CourseId" "GET" -ExpectedStatus "200"

# 18. Create Section
$payload = @{
    title = "Introduction to C# Syntax"
    sequenceOrder = 1
    courseId = $CourseId
} | ConvertTo-Json
$r = Invoke-ApiRequest "Create Section" "/api/Sections" "POST" $payload -Token $InstructorToken -ExpectedStatus "201,200"
$SectionId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $SectionId = $json.id
}

# 19. Get Sections for Course
$r = Invoke-ApiRequest "Get Sections for Course" "/api/Sections/course/$CourseId" "GET" -ExpectedStatus "200"

# 20. Get Specific Section
$r = Invoke-ApiRequest "Get Section by ID" "/api/Sections/$SectionId" "GET" -ExpectedStatus "200"

# 21. Update Section
$payload = @{
    title = "Introduction to C# Core Syntax (Updated)"
    sequenceOrder = 1
} | ConvertTo-Json
$r = Invoke-ApiRequest "Update Section" "/api/Sections/$SectionId" "PUT" $payload -Token $InstructorToken -ExpectedStatus "204,200"

# 22. Create Content (Multipart)
$fields = @{
    Title = "First Steps: Hello World"
    Description = "Setting up .NET SDK and writing your first program"
    SectionId = $SectionId.ToString()
    CourseId = $CourseId.ToString()
    VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
}
$fileFields = @{
    ResourceFileUpload = $ResourcePath
}
$r = Invoke-ApiMultipartRequest "Create Content" "/api/Contents" "POST" $fields $fileFields -Token $InstructorToken -ExpectedStatus "201,200"
$ContentId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $ContentId = $json.id
}

# 23. Get Contents for Section
$r = Invoke-ApiRequest "Get Content for Section" "/api/Contents/section/$SectionId" "GET" -ExpectedStatus "200"

# 24. Get Specific Content
$r = Invoke-ApiRequest "Get Content by ID" "/api/Contents/$ContentId" "GET" -ExpectedStatus "200"

# 25. Update Content (Multipart)
$fields = @{
    Title = "First Steps: Hello World (Updated)"
    Description = "Setting up .NET SDK and building simple apps"
    VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
}
$fileFields = @{}
$r = Invoke-ApiMultipartRequest "Update Content" "/api/Contents/$ContentId" "PUT" $fields $fileFields -Token $InstructorToken -ExpectedStatus "204,200"


# --- DATABASE SEEDING FOR QUIZ ---
Write-Host "Seeding Quiz, Questions, and Answers tied to Course $CourseId and Instructor $InstructorId..." -ForegroundColor Gray
$SqlQueries = "USE $DbName; " +
              "DELETE FROM answers WHERE QuestionId IN (SELECT Id FROM questions WHERE QuizId = 1); " +
              "DELETE FROM questions WHERE QuizId = 1; " +
              "DELETE FROM quizzes WHERE Id = 1; " +
              "INSERT INTO quizzes (Id, CourseId, InstructorId, Status, Marks) VALUES (1, $CourseId, $InstructorId, 'Active', 2); " +
              "INSERT INTO questions (Id, QuizId, QuestionText) VALUES (1, 1, 'What is the main compiler of C#?'), (2, 1, 'Which namespace is used for input/output in C#?'); " +
              "INSERT INTO answers (Id, QuestionId, AnswerText, IsCorrect) VALUES (1, 1, 'Roslyn', 1), (2, 1, 'GCC', 0), (3, 2, 'System.IO', 1), (4, 2, 'System.Net', 0);"

$sqlResult = Execute-SqlQuery $SqlQueries
Write-Host "Seeding complete!" -ForegroundColor Gray


# --- PHASE 3: STUDENT FLOW (CART, PAYMENTS, ENROLLMENTS) ---

# 26. Add Course to Student's Cart
$payload = @{
    courseId = $CourseId
} | ConvertTo-Json
$r = Invoke-ApiRequest "Add Course to Cart" "/api/Cart/add" "POST" $payload -Token $StudentToken -ExpectedStatus "200"

# 27. View Student Cart
$r = Invoke-ApiRequest "View Student Cart" "/api/Cart" "GET" -Token $StudentToken -ExpectedStatus "200"

# 28. Checkout Order
$r = Invoke-ApiRequest "Checkout Order" "/api/Payments/checkout" "POST" -Token $StudentToken -ExpectedStatus "200"
$RazorpayOrderId = "order_test"
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    try {
        $json = $r.ResponseBody | ConvertFrom-Json
        if ($json.razorpayOrderId) {
            $RazorpayOrderId = $json.razorpayOrderId
        }
    } catch {}
}

# Generate HMAC bypass signature using key secret from appsettings
$KeySecret = "IMDJqVnCHOas4EMWjBlFEkiq"
$RazorpayPaymentId = "pay_test_" + (Get-Random)
$PayloadToSign = "$RazorpayOrderId|$RazorpayPaymentId"
$BypassSignature = Get-HmacSha256 -Message $PayloadToSign -Key $KeySecret

Write-Host "Bypass Signature for $PayloadToSign : $BypassSignature" -ForegroundColor Gray

# 29. Verify Payment & Enroll (Bypass)
$payload = @{
    razorpayOrderId = $RazorpayOrderId
    razorpayPaymentId = $RazorpayPaymentId
    razorpaySignature = $BypassSignature
} | ConvertTo-Json
$r = Invoke-ApiRequest "Verify Payment and Enroll" "/api/Payments/verify" "POST" $payload -Token $StudentToken -ExpectedStatus "200"

# 30. Fetch Student Enrollments
$r = Invoke-ApiRequest "Get Student Enrollments" "/api/Enrollments/student/$StudentId" "GET" -ExpectedStatus "200"
$EnrollmentId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    # Find the enrollment for our CourseId
    $myEnrollment = $json | Where-Object { $_.courseId -eq $CourseId }
    if ($myEnrollment) {
        $EnrollmentId = $myEnrollment.id
    }
}

Write-Host "Student Enrollment ID: $EnrollmentId" -ForegroundColor Gray

# 31. Update Progress (Trigger Certificate)
$payload = @{
    enrollmentId = $EnrollmentId
    newProgressPercentage = 100
} | ConvertTo-Json
$r = Invoke-ApiRequest "Update Enrollment Progress (100%)" "/api/Enrollments/update-progress" "POST" $payload -ExpectedStatus "200"

# 32. Submit Course Review
$payload = @{
    courseId = $CourseId
    studentId = $StudentId
    rating = 5
    comment = "Excellent automated test course! 5 stars!"
} | ConvertTo-Json
$r = Invoke-ApiRequest "Submit Course Review" "/api/Reviews/submit" "POST" $payload -ExpectedStatus "200"

# 33. Submit Quiz & Self-Grade
# Quiz ID is seeded as 1. Answer IDs 1 and 3 are correct.
$payload = @{
    quizId = 1
    enrollmentId = $EnrollmentId
    answers = @(
        @{ selectedAnswerId = 1 },
        @{ selectedAnswerId = 3 }
    )
} | ConvertTo-Json
$r = Invoke-ApiRequest "Submit Quiz Submission" "/api/Quizzes/submit" "POST" $payload -ExpectedStatus "200"


# --- PHASE 4: PROFILES & PAYOUTS ---

# 34. Update Profile (User Core)
$payload = @{
    fullName = "Test Student (Updated)"
} | ConvertTo-Json
$r = Invoke-ApiRequest "Update User Profile" "/api/User/profile" "PUT" $payload -Token $StudentToken -ExpectedStatus "200"

# 35. Update Profile (Instructor Details)
$payload = @{
    fullName = "Test Instructor (Updated)"
    headline = "Senior Automation Engineer"
    biography = "Passionate developer specialized in API testing systems."
    profilePictureUrl = "/uploads/profile/test_instructor.jpg"
} | ConvertTo-Json
$r = Invoke-ApiRequest "Update Instructor Details" "/api/Instructor/profile/details" "PUT" $payload -Token $InstructorToken -ExpectedStatus "200"

# 36. Instructor Request Payout
$payload = @{
    amount = 350.00
    bankAccountNumber = "987654321012"
    ifscCode = "HDFC0000123"
} | ConvertTo-Json
$r = Invoke-ApiRequest "Request Instructor Payout" "/api/Payouts/request" "POST" $payload -Token $InstructorToken -ExpectedStatus "200"

# 37. Get Payout History
$r = Invoke-ApiRequest "Get Instructor Payout History" "/api/Payouts/history" "GET" -Token $InstructorToken -ExpectedStatus "200"


# --- PHASE 5: CLEANUP & BOUNDARY DELETIONS ---

# 38. Remove Course from Cart (Verify DELETE Cart Item)
# To test cart deletion, let's add the course again to student cart and then delete it.
$payload = @{
    courseId = $CourseId
} | ConvertTo-Json
$r = Invoke-ApiRequest "Add Course to Cart Again (for Cart Delete test)" "/api/Cart/add" "POST" $payload -Token $StudentToken -ExpectedStatus "200"

$r = Invoke-ApiRequest "Remove Course from Cart" "/api/Cart/remove/$CourseId" "DELETE" -Token $StudentToken -ExpectedStatus "200"

# 39. Delete Course & Section & Content (Clean up temporary ones)
# Let's create a temporary Course, Section, Content to test DELETE endpoints.
$fields = @{
    Title = "Temporary Course for DELETE testing"
    Description = "This course will be deleted"
    Price = "99.00"
    Status = "Draft"
    SubcategoryId = $SubcategoryId.ToString()
}
$fileFields = @{
    ThumbnailImage = $ThumbPath
}
$r = Invoke-ApiMultipartRequest "Create Temporary Course (for DELETE test)" "/api/Courses" "POST" $fields $fileFields -Token $InstructorToken -ExpectedStatus "201"
$TempCourseId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $TempCourseId = $json.courseId
}

$payload = @{
    title = "Temporary Section for DELETE testing"
    sequenceOrder = 1
    courseId = $TempCourseId
} | ConvertTo-Json
$r = Invoke-ApiRequest "Create Temporary Section (for DELETE test)" "/api/Sections" "POST" $payload -Token $InstructorToken -ExpectedStatus "201,200"
$TempSectionId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $TempSectionId = $json.id
}

$fields = @{
    Title = "Temporary Content for DELETE testing"
    Description = "This content will be deleted"
    SectionId = $TempSectionId.ToString()
    CourseId = $TempCourseId.ToString()
    VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
}
$fileFields = @{}
$r = Invoke-ApiMultipartRequest "Create Temporary Content (for DELETE test)" "/api/Contents" "POST" $fields $fileFields -Token $InstructorToken -ExpectedStatus "201,200"
$TempContentId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $TempContentId = $json.id
}

# Now perform DELETEs in order (Content -> Section -> Course)
$r = Invoke-ApiRequest "Delete Content" "/api/Contents/$TempContentId" "DELETE" -Token $InstructorToken -ExpectedStatus "200,204"
$r = Invoke-ApiRequest "Delete Section" "/api/Sections/$TempSectionId" "DELETE" -Token $InstructorToken -ExpectedStatus "200,204"
$r = Invoke-ApiRequest "Delete Course" "/api/Courses/$TempCourseId" "DELETE" -Token $InstructorToken -ExpectedStatus "200,204"

# 40. Delete Category (Verify DELETE Category)
# Let's create a temporary category and delete it
$payload = @{
    name = "Temporary Category for DELETE testing"
    description = "This category will be deleted"
} | ConvertTo-Json
$r = Invoke-ApiRequest "Create Temporary Category (for DELETE test)" "/api/Categories" "POST" $payload -ExpectedStatus "201,200"
$TempCategoryId = 0
if ($r.Status -eq "PASS" -and $r.ResponseBody) {
    $json = $r.ResponseBody | ConvertFrom-Json
    $TempCategoryId = $json.id
}

$r = Invoke-ApiRequest "Delete Category" "/api/Categories/$TempCategoryId" "DELETE" -ExpectedStatus "204,200"


# ==========================================
# TEST SEQUENCE END
# ==========================================

# Clean up local files
if (Test-Path "dummy_thumbnail.jpg") { Remove-Item "dummy_thumbnail.jpg" -Force }
if (Test-Path "dummy_resource.zip") { Remove-Item "dummy_resource.zip" -Force }

# Generate Markdown Report
$ReportFile = "api_test_report.md"

# Helper functions for report formatting
function Get-IndentedString {
    param(
        [string]$Text,
        [int]$Spaces = 4
    )
    if (-not $Text) { return "" }
    $indent = " " * $Spaces
    $lines = $Text -split "\r?\n"
    $indentedLines = foreach ($line in $lines) {
        $indent + $line
    }
    return $indentedLines -join "`n"
}

function Get-FormattedTableBody {
    param([string]$Body)
    if (-not $Body) { return "" }
    
    # Truncate if too long
    if ($Body.Length -gt 120) {
        $Body = $Body.Substring(0, 120) + "..."
    }
    
    # Clean up newlines
    $Body = $Body -replace "\r?\n", " "
    
    # Add spacing for JSON
    if ($Body.StartsWith("{") -or $Body.StartsWith("[")) {
        $Body = $Body -replace '(?<!http|https):', ': ' -replace ',', ', ' -replace '\s+', ' '
    }
    
    # Escape pipe characters for markdown table
    $Body = $Body -replace "\|", "\u007C"
    return $Body
}

$passedCount = ($Results | Where-Object { $_.Status -eq "PASS" }).Count
$failedCount = ($Results | Where-Object { $_.Status -eq "FAIL" }).Count
$totalCount = $Results.Count
$successRate = 0
if ($totalCount -gt 0) {
    $successRate = [Math]::Round(($passedCount / $totalCount) * 100, 2)
}

$tick = '`'
$ticks = '```'
$passEmoji = "$([char]0x2705) PASS"
$failEmoji = "$([char]0x274C) FAIL"

$Markdown = "
# Udemy Clone API Complete Test Report

This report summarizes the programmatically executed test suite targeting all registered endpoints in the Udemy Clone backend. 
The test was run on " + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss') + " against the server running on **$BaseUrl**.

## Summary of Results

| Total Tests | Passed | Failed | Success Rate |
|-------------|--------|--------|--------------|
| " + $totalCount + " | " + $passedCount + " | " + $failedCount + " | " + $successRate + "% |

---

## Detailed Test Logs

| # | Test Name | Method | Endpoint | Expected | Actual | Status |
|---|-----------|--------|----------|----------|--------|--------|
"

$idx = 1
foreach ($res in $Results) {
    $statusEmoji = if ($res.Status -eq "PASS") { $passEmoji } else { $failEmoji }
    $shortBody = Get-FormattedTableBody $res.ResponseBody
    $escEndpoint = $res.Endpoint -replace "\|", "\u007C"
    
    $Markdown += "`n| " + $idx + " | " + $res.Name + " | **" + $res.Method + "** | " + $tick + $escEndpoint + $tick + " | " + $res.StatusCode + " | " + $shortBody + " | **" + $statusEmoji + "** |"
    $idx++
}

$Markdown += "`n`n---`n`n## Technical Details & Payloads`n"

$idx = 1
foreach ($res in $Results) {
    $statusEmoji = if ($res.Status -eq "PASS") { $passEmoji } else { $failEmoji }
    
    $Markdown += "`n### " + $idx + ". " + $res.Name + "`n"
    $Markdown += "*   **Request**: " + $tick + $res.Method + " " + $res.Endpoint + $tick + "`n"
    $Markdown += "*   **Status**: " + $statusEmoji + " (" + $res.StatusCode + ")" + "`n"

    if ($res.RequestPayload) {
        $Markdown += "*   **Payload**:`n"
        $Markdown += (Get-IndentedString ($ticks + "json") 4) + "`n"
        $Markdown += (Get-IndentedString $res.RequestPayload 4) + "`n"
        $Markdown += (Get-IndentedString $ticks 4) + "`n"
    }

    if ($res.ResponseBody) {
        $formattedBody = $res.ResponseBody
        # Try to format json
        try {
            $parsedJson = $res.ResponseBody | ConvertFrom-Json
            $formattedBody = $parsedJson | ConvertTo-Json -Depth 5
        } catch {}

        $Markdown += "*   **Response Body**:`n"
        $Markdown += (Get-IndentedString ($ticks + "json") 4) + "`n"
        $Markdown += (Get-IndentedString $formattedBody 4) + "`n"
        $Markdown += (Get-IndentedString $ticks 4) + "`n"
    }
    $Markdown += "`n---`n"
    $idx++
}

# Write report with UTF-8 encoding (preserving emojis properly on Windows)
[System.IO.File]::WriteAllText($ReportFile, $Markdown, [System.Text.Encoding]::UTF8)
Write-Host "Test Suite Finished! Report generated: $ReportFile" -ForegroundColor Green
