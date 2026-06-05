
# Udemy Clone API Complete Test Report

This report summarizes the programmatically executed test suite targeting all registered endpoints in the Udemy Clone backend. 
The test was run on 2026-06-05 17:28:29 against the server running on **http://localhost:5291**.

## Summary of Results

| Total Tests | Passed | Failed | Success Rate |
|-------------|--------|--------|--------------|
| 48 | 48 | 0 | 100% |

---

## Detailed Test Logs

| # | Test Name | Method | Endpoint | Expected | Actual | Status |
|---|-----------|--------|----------|----------|--------|--------|

| 1 | Register Instructor | **POST** | `/api/Auth/register` | 200 | {"message": "Registration successful. Please check your email for the OTP to verify your account."} | **✅ PASS** |
| 2 | Verify Instructor OTP | **POST** | `/api/Auth/verify-otp` | 200 | {"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltc... | **✅ PASS** |
| 3 | Log In Instructor | **POST** | `/api/Auth/login` | 200 | {"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltc... | **✅ PASS** |
| 4 | Register Student | **POST** | `/api/Auth/register` | 200 | {"message": "Registration successful. Please check your email for the OTP to verify your account."} | **✅ PASS** |
| 5 | Verify Student OTP | **POST** | `/api/Auth/verify-otp` | 200 | {"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltc... | **✅ PASS** |
| 6 | Log In Student | **POST** | `/api/Auth/login` | 200 | {"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltc... | **✅ PASS** |
| 7 | Register Temp User (for Resend OTP test) | **POST** | `/api/Auth/register` | 200 | {"message": "Registration successful. Please check your email for the OTP to verify your account."} | **✅ PASS** |
| 8 | Resend OTP | **POST** | `/api/Auth/resend-otp` | 200 | {"message": "A new OTP has been sent to your email."} | **✅ PASS** |
| 9 | Get Instructor Profile | **GET** | `/api/User/profile` | 200 | {"userId": 16, "fullName": "Test Instructor", "email": "instructor_40895685@example.com", "systemRole": 1, "createdAt": "2026-06-... | **✅ PASS** |
| 10 | Get Student Profile | **GET** | `/api/User/profile` | 200 | {"userId": 17, "fullName": "Test Student", "email": "student_285346765@example.com", "systemRole": 2, "createdAt": "2026-06-05T11... | **✅ PASS** |
| 11 | Create Category | **POST** | `/api/Categories` | 201 | {"id": 8, "name": "Development Test 981452962", "description": "Software development and programming courses for testing"} | **✅ PASS** |
| 12 | Get Categories | **GET** | `/api/Categories` | 200 | [{"id": 1, "name": "Development", "description": "Software development and programming courses"}, {"id": 2, "name": "Development ... | **✅ PASS** |
| 13 | Get Categories Tree | **GET** | `/api/Categories/with-subcategories` | 200 | [{"id": 1, "name": "Development", "description": "Software development and programming courses", "subcategories": [{"id": 1, "nam... | **✅ PASS** |
| 14 | Get Category by ID | **GET** | `/api/Categories/8` | 200 | {"id": 8, "name": "Development Test 981452962", "description": "Software development and programming courses for testing"} | **✅ PASS** |
| 15 | Create Subcategory | **POST** | `/api/Categories/subcategories` | 200 | {"id": 5, "name": "C# & .NET Core Test", "categoryId": 8} | **✅ PASS** |
| 16 | Create Course | **POST** | `/api/Courses` | 201 | {"message": "Course created successfully", "courseId": 9} | **✅ PASS** |
| 17 | Get Courses Catalog | **GET** | `/api/Courses` | 200 | [{"id": 2, "title": "Learn C# from scratch", "description": "Complete frontend thing", "price": 499.00000000000000000000000000, ... | **✅ PASS** |
| 18 | Get Course Details | **GET** | `/api/Courses/9` | 200 | {"id": 9, "title": "Learn C# from Scratch - AutoTest", "description": "Learn C# from scratch with automated verification", "pr... | **✅ PASS** |
| 19 | Create Section | **POST** | `/api/Sections` | 201 | {"id": 9, "title": "Introduction to C# Syntax", "sequenceOrder": 1, "courseId": 9} | **✅ PASS** |
| 20 | Get Sections for Course | **GET** | `/api/Sections/course/9` | 200 | [{"id": 9, "title": "Introduction to C# Syntax", "sequenceOrder": 1, "courseId": 9}] | **✅ PASS** |
| 21 | Get Section by ID | **GET** | `/api/Sections/9` | 200 | {"id": 9, "title": "Introduction to C# Syntax", "sequenceOrder": 1, "courseId": 9} | **✅ PASS** |
| 22 | Update Section | **PUT** | `/api/Sections/9` | 204 |  | **✅ PASS** |
| 23 | Create Content | **POST** | `/api/Contents` | 201 | {"id": 4, "title": "First Steps: Hello World", "description": "Setting up .NET SDK and writing your first program", "filePath"... | **✅ PASS** |
| 24 | Get Content for Section | **GET** | `/api/Contents/section/9` | 200 | [{"id": 4, "title": "First Steps: Hello World", "description": "Setting up .NET SDK and writing your first program", "filePath... | **✅ PASS** |
| 25 | Get Content by ID | **GET** | `/api/Contents/4` | 200 | {"id": 4, "title": "First Steps: Hello World", "description": "Setting up .NET SDK and writing your first program", "filePath"... | **✅ PASS** |
| 26 | Update Content | **PUT** | `/api/Contents/4` | 204 |  | **✅ PASS** |
| 27 | Add Course to Cart | **POST** | `/api/Cart/add` | 200 | {"message": "Course added to cart successfully."} | **✅ PASS** |
| 28 | View Student Cart | **GET** | `/api/Cart` | 200 | [{"id": 7, "courseId": 9, "title": "Learn C# from Scratch - AutoTest", "price": 499.00000000000000000000000000}] | **✅ PASS** |
| 29 | Checkout Order | **POST** | `/api/Payments/checkout` | 200 | {"razorpayOrderId": "order_SxwRZIt2qa7rdq", "amount": 499.00000000000000000000000000, "currency": "INR", "currencySymbol": "₹"} | **✅ PASS** |
| 30 | Verify Payment and Enroll | **POST** | `/api/Payments/verify` | 200 | {"message": "Payment verified and enrollment successful!"} | **✅ PASS** |
| 31 | Get Student Enrollments | **GET** | `/api/Enrollments/student/7` | 200 | [{"id": 4, "studentId": 7, "courseId": 9, "enrolledAt": "2026-06-05T11: 58: 29.193083", "progressPercentage": 0.0000000000000000000... | **✅ PASS** |
| 32 | Update Enrollment Progress (100%) | **POST** | `/api/Enrollments/update-progress` | 200 | {"id": 4, "studentId": 7, "courseId": 9, "enrolledAt": "2026-06-05T11: 58: 29.193083", "progressPercentage": 100, "student": null, "co... | **✅ PASS** |
| 33 | Submit Course Review | **POST** | `/api/Reviews/submit` | 200 | Review submitted successfully! | **✅ PASS** |
| 34 | Submit Quiz Submission | **POST** | `/api/Quizzes/submit` | 200 | {"quizId": 1, "totalScore": 2, "passed": false} | **✅ PASS** |
| 35 | Update User Profile | **PUT** | `/api/User/profile` | 200 | {"message": "Core account name updated successfully."} | **✅ PASS** |
| 36 | Update Instructor Details | **PUT** | `/api/Instructor/profile/details` | 200 | {"message": "Instructor bio details updated successfully."} | **✅ PASS** |
| 37 | Request Instructor Payout | **POST** | `/api/Payouts/request` | 200 | {"message": "Payout successfully completed and logged."} | **✅ PASS** |
| 38 | Get Instructor Payout History | **GET** | `/api/Payouts/history` | 200 | [{"id": 4, "instructorId": 8, "amountPaid": 350.00000000000000000000000000, "status": "Processed", "payoutDate": "2026-06-05T11: 5... | **✅ PASS** |
| 39 | Add Course to Cart Again (for Cart Delete test) | **POST** | `/api/Cart/add` | 200 | {"message": "Course added to cart successfully."} | **✅ PASS** |
| 40 | Remove Course from Cart | **DELETE** | `/api/Cart/remove/9` | 200 | {"message": "Course removed from cart successfully."} | **✅ PASS** |
| 41 | Create Temporary Course (for DELETE test) | **POST** | `/api/Courses` | 201 | {"message": "Course created successfully", "courseId": 10} | **✅ PASS** |
| 42 | Create Temporary Section (for DELETE test) | **POST** | `/api/Sections` | 201 | {"id": 10, "title": "Temporary Section for DELETE testing", "sequenceOrder": 1, "courseId": 10} | **✅ PASS** |
| 43 | Create Temporary Content (for DELETE test) | **POST** | `/api/Contents` | 201 | {"id": 5, "title": "Temporary Content for DELETE testing", "description": "This content will be deleted", "filePath": "", "video... | **✅ PASS** |
| 44 | Delete Content | **DELETE** | `/api/Contents/5` | 204 |  | **✅ PASS** |
| 45 | Delete Section | **DELETE** | `/api/Sections/10` | 204 |  | **✅ PASS** |
| 46 | Delete Course | **DELETE** | `/api/Courses/10` | 204 |  | **✅ PASS** |
| 47 | Create Temporary Category (for DELETE test) | **POST** | `/api/Categories` | 201 | {"id": 9, "name": "Temporary Category for DELETE testing", "description": "This category will be deleted"} | **✅ PASS** |
| 48 | Delete Category | **DELETE** | `/api/Categories/9` | 204 |  | **✅ PASS** |

---

## Technical Details & Payloads

### 1. Register Instructor
*   **Request**: `POST /api/Auth/register`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "email":  "instructor_40895685@example.com",
        "systemRole":  1,
        "fullName":  "Test Instructor",
        "password":  "SecurePassword123"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Registration successful. Please check your email for the OTP to verify your account."
    }
    ```

---

### 2. Verify Instructor OTP
*   **Request**: `POST /api/Auth/verify-otp`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "email":  "instructor_40895685@example.com",
        "otp":  "678266"
    }
    ```
*   **Response Body**:
    ```json
    {
        "token":  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE2IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiaW5zdHJ1Y3Rvcl80MDg5NTY4NUBleGFtcGxlLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IjEiLCJJbnN0cnVjdG9ySWQiOiI4IiwiZXhwIjoxNzgwNjY0MjkyLCJpc3MiOiJVZGVteUNsb25lQmFja2VuZCIsImF1ZCI6IlVkZW15Q2xvbmVGcm9udGVuZCJ9.taT9M76Zq_1YrEsfPFsaREvW4yzIhwJtyrd4sa-N6vk",
        "fullName":  "Test Instructor",
        "email":  "instructor_40895685@example.com",
        "systemRole":  1
    }
    ```

---

### 3. Log In Instructor
*   **Request**: `POST /api/Auth/login`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "password":  "SecurePassword123",
        "email":  "instructor_40895685@example.com"
    }
    ```
*   **Response Body**:
    ```json
    {
        "token":  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE2IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiaW5zdHJ1Y3Rvcl80MDg5NTY4NUBleGFtcGxlLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IjEiLCJJbnN0cnVjdG9ySWQiOiI4IiwiZXhwIjoxNzgwNjY0MjkyLCJpc3MiOiJVZGVteUNsb25lQmFja2VuZCIsImF1ZCI6IlVkZW15Q2xvbmVGcm9udGVuZCJ9.taT9M76Zq_1YrEsfPFsaREvW4yzIhwJtyrd4sa-N6vk",
        "fullName":  "Test Instructor",
        "email":  "instructor_40895685@example.com",
        "systemRole":  1
    }
    ```

---

### 4. Register Student
*   **Request**: `POST /api/Auth/register`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "email":  "student_285346765@example.com",
        "systemRole":  2,
        "fullName":  "Test Student",
        "password":  "SecurePassword123"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Registration successful. Please check your email for the OTP to verify your account."
    }
    ```

---

### 5. Verify Student OTP
*   **Request**: `POST /api/Auth/verify-otp`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "email":  "student_285346765@example.com",
        "otp":  "885897"
    }
    ```
*   **Response Body**:
    ```json
    {
        "token":  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoic3R1ZGVudF8yODUzNDY3NjVAZXhhbXBsZS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiIyIiwiZXhwIjoxNzgwNjY0Mjk4LCJpc3MiOiJVZGVteUNsb25lQmFja2VuZCIsImF1ZCI6IlVkZW15Q2xvbmVGcm9udGVuZCJ9.WAUrP5ltjW4G97AfNDXctEmPVUukzPjr8AK0_GmPBPE",
        "fullName":  "Test Student",
        "email":  "student_285346765@example.com",
        "systemRole":  2
    }
    ```

---

### 6. Log In Student
*   **Request**: `POST /api/Auth/login`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "password":  "SecurePassword123",
        "email":  "student_285346765@example.com"
    }
    ```
*   **Response Body**:
    ```json
    {
        "token":  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE3IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoic3R1ZGVudF8yODUzNDY3NjVAZXhhbXBsZS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiIyIiwiZXhwIjoxNzgwNjY0Mjk4LCJpc3MiOiJVZGVteUNsb25lQmFja2VuZCIsImF1ZCI6IlVkZW15Q2xvbmVGcm9udGVuZCJ9.WAUrP5ltjW4G97AfNDXctEmPVUukzPjr8AK0_GmPBPE",
        "fullName":  "Test Student",
        "email":  "student_285346765@example.com",
        "systemRole":  2
    }
    ```

---

### 7. Register Temp User (for Resend OTP test)
*   **Request**: `POST /api/Auth/register`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "email":  "temp_996759687@example.com",
        "systemRole":  2,
        "fullName":  "Temp User",
        "password":  "SecurePassword123"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Registration successful. Please check your email for the OTP to verify your account."
    }
    ```

---

### 8. Resend OTP
*   **Request**: `POST /api/Auth/resend-otp`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "email":  "temp_996759687@example.com"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "A new OTP has been sent to your email."
    }
    ```

---

### 9. Get Instructor Profile
*   **Request**: `GET /api/User/profile`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "userId":  16,
        "fullName":  "Test Instructor",
        "email":  "instructor_40895685@example.com",
        "systemRole":  1,
        "createdAt":  "2026-06-05T11:58:06.572337",
        "studentDetails":  null,
        "instructorDetails":  {
                                  "instructorId":  8,
                                  "headline":  null,
                                  "biography":  null,
                                  "profilePictureUrl":  ""
                              }
    }
    ```

---

### 10. Get Student Profile
*   **Request**: `GET /api/User/profile`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "userId":  17,
        "fullName":  "Test Student",
        "email":  "student_285346765@example.com",
        "systemRole":  2,
        "createdAt":  "2026-06-05T11:58:12.444418",
        "studentDetails":  {
                               "studentId":  7
                           },
        "instructorDetails":  null
    }
    ```

---

### 11. Create Category
*   **Request**: `POST /api/Categories`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    {
        "name":  "Development Test 981452962",
        "description":  "Software development and programming courses for testing"
    }
    ```
*   **Response Body**:
    ```json
    {
        "id":  8,
        "name":  "Development Test 981452962",
        "description":  "Software development and programming courses for testing"
    }
    ```

---

### 12. Get Categories
*   **Request**: `GET /api/Categories`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    [
        {
            "id":  1,
            "name":  "Development",
            "description":  "Software development and programming courses"
        },
        {
            "id":  2,
            "name":  "Development Test 137804355",
            "description":  "Software development and programming courses for testing"
        },
        {
            "id":  4,
            "name":  "Development Test 1904410820",
            "description":  "Software development and programming courses for testing"
        },
        {
            "id":  6,
            "name":  "Development Test 361030517",
            "description":  "Software development and programming courses for testing"
        },
        {
            "id":  8,
            "name":  "Development Test 981452962",
            "description":  "Software development and programming courses for testing"
        }
    ]
    ```

---

### 13. Get Categories Tree
*   **Request**: `GET /api/Categories/with-subcategories`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    [
        {
            "id":  1,
            "name":  "Development",
            "description":  "Software development and programming courses",
            "subcategories":  [
                                  {
                                      "id":  1,
                                      "name":  "Frontend Development - Angular",
                                      "categoryId":  1
                                  }
                              ]
        },
        {
            "id":  2,
            "name":  "Development Test 137804355",
            "description":  "Software development and programming courses for testing",
            "subcategories":  [
                                  {
                                      "id":  2,
                                      "name":  "C# \u0026 .NET Core Test",
                                      "categoryId":  2
                                  }
                              ]
        },
        {
            "id":  4,
            "name":  "Development Test 1904410820",
            "description":  "Software development and programming courses for testing",
            "subcategories":  [
                                  {
                                      "id":  3,
                                      "name":  "C# \u0026 .NET Core Test",
                                      "categoryId":  4
                                  }
                              ]
        },
        {
            "id":  6,
            "name":  "Development Test 361030517",
            "description":  "Software development and programming courses for testing",
            "subcategories":  [
                                  {
                                      "id":  4,
                                      "name":  "C# \u0026 .NET Core Test",
                                      "categoryId":  6
                                  }
                              ]
        },
        {
            "id":  8,
            "name":  "Development Test 981452962",
            "description":  "Software development and programming courses for testing",
            "subcategories":  [
    
                              ]
        }
    ]
    ```

---

### 14. Get Category by ID
*   **Request**: `GET /api/Categories/8`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  8,
        "name":  "Development Test 981452962",
        "description":  "Software development and programming courses for testing"
    }
    ```

---

### 15. Create Subcategory
*   **Request**: `POST /api/Categories/subcategories`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "categoryId":  8,
        "name":  "C# \u0026 .NET Core Test"
    }
    ```
*   **Response Body**:
    ```json
    {
        "id":  5,
        "name":  "C# \u0026 .NET Core Test",
        "categoryId":  8
    }
    ```

---

### 16. Create Course
*   **Request**: `POST /api/Courses`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    Multipart Form Data: {"Price":"499.00","Title":"Learn C# from Scratch - AutoTest","Description":"Learn C# from scratch with automated verification","SubcategoryId":"5","Status":"Premium"}
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Course created successfully",
        "courseId":  9
    }
    ```

---

### 17. Get Courses Catalog
*   **Request**: `GET /api/Courses`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    [
        {
            "id":  2,
            "title":  "Learn C# from scratch",
            "description":  "Complete frontend thing",
            "price":  499.00000000000000000000000000,
            "thumbnailUrl":  "/Uploads/Courses/57e8e7ed-8f8d-4f1c-99ae-95c4148aa4f6_Signature.jpg",
            "status":  "Premium",
            "isQuiz":  false,
            "instructorName":  "Amith Vanam",
            "subcategoryName":  "Frontend Development - Angular"
        },
        {
            "id":  3,
            "title":  "Learn C# from Scratch - AutoTest",
            "description":  "Learn C# from scratch with automated verification",
            "price":  499.00000000000000000000000000,
            "thumbnailUrl":  "/Uploads/Courses/b5a95d3f-d189-4746-b440-9c5f08c1fb8b_dummy_thumbnail.jpg",
            "status":  "Premium",
            "isQuiz":  false,
            "instructorName":  "Test Instructor",
            "subcategoryName":  "C# \u0026 .NET Core Test"
        },
        {
            "id":  5,
            "title":  "Learn C# from Scratch - AutoTest",
            "description":  "Learn C# from scratch with automated verification",
            "price":  499.00000000000000000000000000,
            "thumbnailUrl":  "/Uploads/Courses/2e4aba25-32be-4187-95bd-6c75be518326_dummy_thumbnail.jpg",
            "status":  "Premium",
            "isQuiz":  false,
            "instructorName":  "Test Instructor",
            "subcategoryName":  "C# \u0026 .NET Core Test"
        },
        {
            "id":  7,
            "title":  "Learn C# from Scratch - AutoTest",
            "description":  "Learn C# from scratch with automated verification",
            "price":  499.00000000000000000000000000,
            "thumbnailUrl":  "/Uploads/Courses/52f06665-f62b-493d-af25-fe2c25a8ba51_dummy_thumbnail.jpg",
            "status":  "Premium",
            "isQuiz":  false,
            "instructorName":  "Test Instructor",
            "subcategoryName":  "C# \u0026 .NET Core Test"
        },
        {
            "id":  9,
            "title":  "Learn C# from Scratch - AutoTest",
            "description":  "Learn C# from scratch with automated verification",
            "price":  499.00000000000000000000000000,
            "thumbnailUrl":  "/Uploads/Courses/a3d30304-be69-41bb-910f-3a5edd7bb029_dummy_thumbnail.jpg",
            "status":  "Premium",
            "isQuiz":  false,
            "instructorName":  "Test Instructor",
            "subcategoryName":  "C# \u0026 .NET Core Test"
        }
    ]
    ```

---

### 18. Get Course Details
*   **Request**: `GET /api/Courses/9`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  9,
        "title":  "Learn C# from Scratch - AutoTest",
        "description":  "Learn C# from scratch with automated verification",
        "price":  499.00000000000000000000000000,
        "thumbnailUrl":  "/Uploads/Courses/a3d30304-be69-41bb-910f-3a5edd7bb029_dummy_thumbnail.jpg",
        "status":  "Premium",
        "isQuiz":  false,
        "instructorId":  8,
        "instructorName":  "Test Instructor",
        "instructorBio":  null,
        "subcategoryName":  "C# \u0026 .NET Core Test",
        "sections":  [
    
                     ]
    }
    ```

---

### 19. Create Section
*   **Request**: `POST /api/Sections`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    {
        "title":  "Introduction to C# Syntax",
        "sequenceOrder":  1,
        "courseId":  9
    }
    ```
*   **Response Body**:
    ```json
    {
        "id":  9,
        "title":  "Introduction to C# Syntax",
        "sequenceOrder":  1,
        "courseId":  9
    }
    ```

---

### 20. Get Sections for Course
*   **Request**: `GET /api/Sections/course/9`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  9,
        "title":  "Introduction to C# Syntax",
        "sequenceOrder":  1,
        "courseId":  9
    }
    ```

---

### 21. Get Section by ID
*   **Request**: `GET /api/Sections/9`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  9,
        "title":  "Introduction to C# Syntax",
        "sequenceOrder":  1,
        "courseId":  9
    }
    ```

---

### 22. Update Section
*   **Request**: `PUT /api/Sections/9`
*   **Status**: ✅ PASS (204)
*   **Payload**:
    ```json
    {
        "sequenceOrder":  1,
        "title":  "Introduction to C# Core Syntax (Updated)"
    }
    ```

---

### 23. Create Content
*   **Request**: `POST /api/Contents`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    Multipart Form Data: {"Title":"First Steps: Hello World","Description":"Setting up .NET SDK and writing your first program","VideoUrl":"https://www.youtube.com/watch?v=dQw4w9WgXcQ","CourseId":"9","SectionId":"9"}
    ```
*   **Response Body**:
    ```json
    {
        "id":  4,
        "title":  "First Steps: Hello World",
        "description":  "Setting up .NET SDK and writing your first program",
        "filePath":  "/Uploads/Resources/3a05fef4-e4b9-4d25-9122-acf4876602a6_dummy_resource.zip",
        "videoUrl":  "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
        "sectionId":  9,
        "courseId":  9
    }
    ```

---

### 24. Get Content for Section
*   **Request**: `GET /api/Contents/section/9`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  4,
        "title":  "First Steps: Hello World",
        "description":  "Setting up .NET SDK and writing your first program",
        "filePath":  "/Uploads/Resources/3a05fef4-e4b9-4d25-9122-acf4876602a6_dummy_resource.zip",
        "videoUrl":  "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
        "sectionId":  9,
        "courseId":  9
    }
    ```

---

### 25. Get Content by ID
*   **Request**: `GET /api/Contents/4`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  4,
        "title":  "First Steps: Hello World",
        "description":  "Setting up .NET SDK and writing your first program",
        "filePath":  "/Uploads/Resources/3a05fef4-e4b9-4d25-9122-acf4876602a6_dummy_resource.zip",
        "videoUrl":  "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
        "sectionId":  9,
        "courseId":  9
    }
    ```

---

### 26. Update Content
*   **Request**: `PUT /api/Contents/4`
*   **Status**: ✅ PASS (204)
*   **Payload**:
    ```json
    Multipart Form Data: {"Title":"First Steps: Hello World (Updated)","Description":"Setting up .NET SDK and building simple apps","VideoUrl":"https://www.youtube.com/watch?v=dQw4w9WgXcQ"}
    ```

---

### 27. Add Course to Cart
*   **Request**: `POST /api/Cart/add`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "courseId":  9
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Course added to cart successfully."
    }
    ```

---

### 28. View Student Cart
*   **Request**: `GET /api/Cart`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  7,
        "courseId":  9,
        "title":  "Learn C# from Scratch - AutoTest",
        "price":  499.00000000000000000000000000
    }
    ```

---

### 29. Checkout Order
*   **Request**: `POST /api/Payments/checkout`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "razorpayOrderId":  "order_SxwRZIt2qa7rdq",
        "amount":  499.00000000000000000000000000,
        "currency":  "INR",
        "currencySymbol":  "₹"
    }
    ```

---

### 30. Verify Payment and Enroll
*   **Request**: `POST /api/Payments/verify`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "razorpaySignature":  "962f274e8d984968d2f9ca86ed3172a84d0723b171e7851d82c2cb91e223fc92",
        "razorpayOrderId":  "order_SxwRZIt2qa7rdq",
        "razorpayPaymentId":  "pay_test_1474529339"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Payment verified and enrollment successful!"
    }
    ```

---

### 31. Get Student Enrollments
*   **Request**: `GET /api/Enrollments/student/7`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  4,
        "studentId":  7,
        "courseId":  9,
        "enrolledAt":  "2026-06-05T11:58:29.193083",
        "progressPercentage":  0.0000000000000000000000000000,
        "student":  null,
        "course":  {
                       "id":  9,
                       "title":  "Learn C# from Scratch - AutoTest",
                       "description":  "Learn C# from scratch with automated verification",
                       "price":  499.00000000000000000000000000,
                       "thumbnailUrl":  "/Uploads/Courses/a3d30304-be69-41bb-910f-3a5edd7bb029_dummy_thumbnail.jpg",
                       "status":  "Premium",
                       "isQuiz":  false,
                       "instructorId":  8,
                       "subcategoryId":  5,
                       "instructor":  null,
                       "subcategory":  null,
                       "sections":  null,
                       "contents":  null,
                       "quizzes":  null
                   }
    }
    ```

---

### 32. Update Enrollment Progress (100%)
*   **Request**: `POST /api/Enrollments/update-progress`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "enrollmentId":  4,
        "newProgressPercentage":  100
    }
    ```
*   **Response Body**:
    ```json
    {
        "id":  4,
        "studentId":  7,
        "courseId":  9,
        "enrolledAt":  "2026-06-05T11:58:29.193083",
        "progressPercentage":  100,
        "student":  null,
        "course":  null
    }
    ```

---

### 33. Submit Course Review
*   **Request**: `POST /api/Reviews/submit`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "rating":  5,
        "comment":  "Excellent automated test course! 5 stars!",
        "studentId":  7,
        "courseId":  9
    }
    ```
*   **Response Body**:
    ```json
    Review submitted successfully!
    ```

---

### 34. Submit Quiz Submission
*   **Request**: `POST /api/Quizzes/submit`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "answers":  [
                        {
                            "selectedAnswerId":  1
                        },
                        {
                            "selectedAnswerId":  3
                        }
                    ],
        "quizId":  1,
        "enrollmentId":  4
    }
    ```
*   **Response Body**:
    ```json
    {
        "quizId":  1,
        "totalScore":  2,
        "passed":  false
    }
    ```

---

### 35. Update User Profile
*   **Request**: `PUT /api/User/profile`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "fullName":  "Test Student (Updated)"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Core account name updated successfully."
    }
    ```

---

### 36. Update Instructor Details
*   **Request**: `PUT /api/Instructor/profile/details`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "profilePictureUrl":  "/uploads/profile/test_instructor.jpg",
        "fullName":  "Test Instructor (Updated)",
        "biography":  "Passionate developer specialized in API testing systems.",
        "headline":  "Senior Automation Engineer"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Instructor bio details updated successfully."
    }
    ```

---

### 37. Request Instructor Payout
*   **Request**: `POST /api/Payouts/request`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "amount":  350,
        "ifscCode":  "HDFC0000123",
        "bankAccountNumber":  "987654321012"
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Payout successfully completed and logged."
    }
    ```

---

### 38. Get Instructor Payout History
*   **Request**: `GET /api/Payouts/history`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "id":  4,
        "instructorId":  8,
        "amountPaid":  350.00000000000000000000000000,
        "status":  "Processed",
        "payoutDate":  "2026-06-05T11:58:29.39129",
        "instructor":  {
                           "id":  8,
                           "userId":  16,
                           "biography":  "Passionate developer specialized in API testing systems.",
                           "websiteLink":  null,
                           "facebookLink":  null,
                           "xLink":  null,
                           "instagramLink":  null,
                           "linkedInLink":  null,
                           "gitHubLink":  null,
                           "isPublic":  false,
                           "bankAccountNumber":  "987654321012",
                           "ifscCode":  "HDFC0000123",
                           "department":  null,
                           "user":  null,
                           "courses":  null,
                           "quizzes":  null,
                           "payoutHistories":  [
                                                   null
                                               ]
                       }
    }
    ```

---

### 39. Add Course to Cart Again (for Cart Delete test)
*   **Request**: `POST /api/Cart/add`
*   **Status**: ✅ PASS (200)
*   **Payload**:
    ```json
    {
        "courseId":  9
    }
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Course added to cart successfully."
    }
    ```

---

### 40. Remove Course from Cart
*   **Request**: `DELETE /api/Cart/remove/9`
*   **Status**: ✅ PASS (200)
*   **Response Body**:
    ```json
    {
        "message":  "Course removed from cart successfully."
    }
    ```

---

### 41. Create Temporary Course (for DELETE test)
*   **Request**: `POST /api/Courses`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    Multipart Form Data: {"Price":"99.00","Title":"Temporary Course for DELETE testing","Description":"This course will be deleted","SubcategoryId":"5","Status":"Draft"}
    ```
*   **Response Body**:
    ```json
    {
        "message":  "Course created successfully",
        "courseId":  10
    }
    ```

---

### 42. Create Temporary Section (for DELETE test)
*   **Request**: `POST /api/Sections`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    {
        "title":  "Temporary Section for DELETE testing",
        "sequenceOrder":  1,
        "courseId":  10
    }
    ```
*   **Response Body**:
    ```json
    {
        "id":  10,
        "title":  "Temporary Section for DELETE testing",
        "sequenceOrder":  1,
        "courseId":  10
    }
    ```

---

### 43. Create Temporary Content (for DELETE test)
*   **Request**: `POST /api/Contents`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    Multipart Form Data: {"Title":"Temporary Content for DELETE testing","Description":"This content will be deleted","VideoUrl":"https://www.youtube.com/watch?v=dQw4w9WgXcQ","CourseId":"10","SectionId":"10"}
    ```
*   **Response Body**:
    ```json
    {
        "id":  5,
        "title":  "Temporary Content for DELETE testing",
        "description":  "This content will be deleted",
        "filePath":  "",
        "videoUrl":  "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
        "sectionId":  10,
        "courseId":  10
    }
    ```

---

### 44. Delete Content
*   **Request**: `DELETE /api/Contents/5`
*   **Status**: ✅ PASS (204)

---

### 45. Delete Section
*   **Request**: `DELETE /api/Sections/10`
*   **Status**: ✅ PASS (204)

---

### 46. Delete Course
*   **Request**: `DELETE /api/Courses/10`
*   **Status**: ✅ PASS (204)

---

### 47. Create Temporary Category (for DELETE test)
*   **Request**: `POST /api/Categories`
*   **Status**: ✅ PASS (201)
*   **Payload**:
    ```json
    {
        "name":  "Temporary Category for DELETE testing",
        "description":  "This category will be deleted"
    }
    ```
*   **Response Body**:
    ```json
    {
        "id":  9,
        "name":  "Temporary Category for DELETE testing",
        "description":  "This category will be deleted"
    }
    ```

---

### 48. Delete Category
*   **Request**: `DELETE /api/Categories/9`
*   **Status**: ✅ PASS (204)

---
