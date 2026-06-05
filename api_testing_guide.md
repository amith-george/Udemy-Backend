# Udemy Clone API - Complete Backend Testing Guide

This guide maps out the entire set of API endpoints in the backend and provides the exact sequence, HTTP methods, authorization requirements, SQL setup scripts, and JSON payloads needed to execute a complete end-to-end testing flow.

---

## Prerequisites & Database Seeding

Because some features (like Subcategories and Quizzes) do not have creation endpoints, you must seed these directly in your database before testing.

### 1. Database Connection Details
*   **Host**: `localhost`
*   **Port**: `3306`
*   **Database**: `udemy_db`
*   **Username**: `root`
*   **Password**: `root`
*(as configured in [appsettings.json](file:///d:/DevOps/Udemy/Udemy-Backend/appsettings.json))*

### 2. SQL Seeding script
Connect to your database via MySQL workbench or command line and run the following script:

```sql
-- Use the database
USE udemy_db;

-- 1. Create a Subcategory (since there is no Subcategory creation endpoint)
-- Insert a Category first (or do it via API, let's assume Category ID = 1)
INSERT INTO Categories (Id, Name, Description) 
VALUES (1, 'Development', 'Software development and programming courses')
ON DUPLICATE KEY UPDATE Name=VALUES(Name);

INSERT INTO Subcategories (Id, Name, CategoryId) 
VALUES (1, 'C# & .NET Core', 1)
ON DUPLICATE KEY UPDATE Name=VALUES(Name);

-- 2. Create a Quiz, Questions, and Answers (since there are no creation endpoints)
-- Assume CourseId = 1 and InstructorId = 1 will be generated when you create them in the flow.
-- If your auto-incremented IDs are different, adjust these values.
INSERT INTO Quizzes (Id, CourseId, InstructorId, Status, Marks) 
VALUES (1, 1, 1, 'Active', 2)
ON DUPLICATE KEY UPDATE Status=VALUES(Status);

-- Questions
INSERT INTO Questions (Id, QuizId, QuestionText) 
VALUES (1, 1, 'What is the main compiler of C#?')
ON DUPLICATE KEY UPDATE QuestionText=VALUES(QuestionText);

INSERT INTO Questions (Id, QuizId, QuestionText) 
VALUES (2, 1, 'Which namespace is used for input/output in C#?')
ON DUPLICATE KEY UPDATE QuestionText=VALUES(QuestionText);

-- Answers
INSERT INTO Answers (Id, QuestionId, AnswerText, IsCorrect) 
VALUES (1, 1, 'Roslyn', 1),
       (2, 1, 'GCC', 0),
       (3, 2, 'System.IO', 1),
       (4, 2, 'System.Net', 0)
ON DUPLICATE KEY UPDATE AnswerText=VALUES(AnswerText), IsCorrect=VALUES(IsCorrect);
```

---

## Phase 1: Authentication & User Setup

All protected endpoints require the `Authorization` header containing the JWT token:
`Authorization: Bearer <your_jwt_token>`

### 1. Register Instructor Account
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Auth/register`
*   **Auth**: None (Public)
*   **JSON Payload**:
    ```json
    {
      "fullName": "Jane Doe (Instructor)",
      "email": "instructor@example.com",
      "password": "SecurePassword123",
      "systemRole": 1
    }
    ```
*   **Expected Response**: `200 OK`
    ```json
    {
      "message": "Registration successful. Please check your email for the OTP to verify your account."
    }
    ```

### 2. Verify Instructor OTP
Since email verification is enabled, get the OTP sent to `instructor@example.com` or find it in your database using:
`SELECT otp FROM Users WHERE Email = 'instructor@example.com';`
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Auth/verify-otp`
*   **Auth**: None (Public)
*   **JSON Payload**:
    ```json
    {
      "email": "instructor@example.com",
      "otp": "123456" 
    }
    ```
*   **Expected Response**: `200 OK` (Returns the JWT token). Note the token value.

### 3. Log In Instructor (Alternative/Verification)
If your token expires or you need to authenticate again:
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Auth/login`
*   **Auth**: None (Public)
*   **JSON Payload**:
    ```json
    {
      "email": "instructor@example.com",
      "password": "SecurePassword123"
    }
    ```
*   **Expected Response**: `200 OK` returning the JWT `token`.

---

## Phase 2: Category & Course Administration

These endpoints are managed by the Instructor or Admin.

### 4. Create Category (API Option)
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Categories`
*   **Auth**: None
*   **JSON Payload**:
    ```json
    {
      "name": "Development",
      "description": "Software development and programming courses"
    }
    ```

### 4.1 Create Subcategory
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Categories/subcategories`
*   **Auth**: None
*   **JSON Payload**:
    ```json
    {
      "name": "C# & .NET Core",
      "categoryId": 1
    }
    ```

### 5. Create Course (Form-Data)
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Courses`
*   **Auth**: Bearer Token (Instructor)
*   **Content-Type**: `multipart/form-data`
*   **Form-Data Fields**:
    *   `Title`: `Learn C# from Scratch`
    *   `Description`: `Learn C# from Scratcht`
    *   `Price`: `499.00`
    *   `Status`: `Premium`
    *   `SubcategoryId`: `1`
    *   `ThumbnailImage`: `[Select any image file to upload]`
*   **Expected Response**: `201 Created`
    ```json
    {
      "message": "Course created successfully",
      "courseId": 1
    }
    ```

### 6. Create Section (Syllabus Module)
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Sections`
*   **Auth**: Bearer Token (Instructor)
*   **JSON Payload**:
    ```json
    {
      "title": "Introduction to C# Syntax",
      "sequenceOrder": 1,
      "courseId": 1
    }
    ```

### 7. Create Content (Video Lecture / File)
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Contents`
*   **Auth**: Bearer Token (Instructor)
*   **Content-Type**: `multipart/form-data`
*   **Form-Data Fields**:
    *   `Title`: `First Steps: Hello World`
    *   `Description`: `Setting up .NET SDK and writing your first program`
    *   `SectionId`: `1`
    *   `CourseId`: `1`
    *   `VideoUrl`: `https://www.youtube.com/watch?v=dQw4w9WgXcQ` *(Optional if uploading a file instead)*
    *   `VideoUpload`: `[Optional: Select an mp4 file to upload]`
    *   `ResourceFileUpload`: `[Optional: Select a zip or pdf resource file]`

---

## Phase 3: Student Flow (Registration & Cart)

Now test the student purchasing and learning experience.

### 8. Register Student Account
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Auth/register`
*   **Auth**: None (Public)
*   **JSON Payload**:
    ```json
    {
      "fullName": "John Doe (Student)",
      "email": "student@example.com",
      "password": "SecurePassword123",
      "systemRole": 2
    }
    ```

### 9. Verify Student OTP
Verify via OTP sent to email, or querying the DB:
`SELECT otp FROM Users WHERE Email = 'student@example.com';`
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Auth/verify-otp`
*   **Auth**: None
*   **JSON Payload**:
    ```json
    {
      "email": "student@example.com",
      "otp": "123456"
    }
    ```
*(Copy the Student JWT token returning in this step!)*

### 10. Add Course to Student's Cart
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Cart/add`
*   **Auth**: Bearer Token (Student)
*   **JSON Payload**:
    ```json
    {
      "courseId": 1
    }
    ```

### 11. View Student Cart
*   **Method**: `GET`
*   **URL**: `http://localhost:5000/api/Cart`
*   **Auth**: Bearer Token (Student)

---

## Phase 4: Purchase & Enrollment (Razorpay Bypass)

This phase tests checkout, payment verification, and automatic course enrollment.

### 12. Checkout Order
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Payments/checkout`
*   **Auth**: Bearer Token (Student)
*   **Expected Response**: Returns a `razorpayOrderId`.

### 13. Verify Payment & Enroll (Bypass)
The system verifies the HMAC-SHA256 signature of the payload `${OrderId}|${PaymentId}` using your configured secret key.
To bypass using real Razorpay servers, use this pre-calculated signature match:
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Payments/verify`
*   **Auth**: Bearer Token (Student)
*   **JSON Payload**:
    ```json
    {
      "razorpayOrderId": "order_test",
      "razorpayPaymentId": "pay_test",
      "razorpaySignature": "c7d06b7adf3f37216b8591995ea92f8d8bbd24719e034a21558a03c10cad0920"
    }
    ```
*   **Expected Response**: `200 OK`
    ```json
    {
      "message": "Payment verified and enrollment successful!"
    }
    ```

---

## Phase 5: Learning Experience & Assessments

### 14. Fetch Student Enrollments
Get the newly created enrollment record.
*   **Method**: `GET`
*   **URL**: `http://localhost:5000/api/Enrollments/student/1` *(Replace 1 with the Student's profile ID)*
*   **Auth**: None
*   **Expected Response**: Contains details of enrolled courses, including the `enrollmentId` (e.g., `1`).

### 15. Update Progress (Trigger Certificate)
Update course progress. Passing `100` triggers automatic certificate creation.
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Enrollments/update-progress`
*   **Auth**: None
*   **JSON Payload**:
    ```json
    {
      "enrollmentId": 1,
      "newProgressPercentage": 100
    }
    ```

### 16. Submit Course Review
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Reviews/submit`
*   **Auth**: None
*   **JSON Payload**:
    ```json
    {
      "courseId": 1,
      "studentId": 1,
      "rating": 5,
      "comment": "Absolutely fantastic course! Highly recommended!"
    }
    ```

### 17. Submit Quiz & Self-Grade
Submit the quiz (assuming you ran the SQL seeding script for Quiz ID = 1 and Question IDs 1 and 2, which have correct answers 1 and 3 respectively).
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Quizzes/submit`
*   **Auth**: None
*   **JSON Payload**:
    ```json
    {
      "quizId": 1,
      "enrollmentId": 1,
      "answers": [
        {
          "selectedAnswerId": 1
        },
        {
          "selectedAnswerId": 3
        }
      ]
    }
    ```
*   **Expected Response**:
    ```json
    {
      "quizId": 1,
      "totalScore": 2
    }
    ```

---

## Phase 6: Payouts & Miscellaneous

### 18. Update Profiles
*   **Method**: `PUT`
*   **URL**: `http://localhost:5000/api/User/profile`
*   **Auth**: Bearer Token
*   **JSON Payload**:
    ```json
    {
      "fullName": "Jane Doe (Certified Instructor)"
    }
    ```

*   **Method**: `PUT`
*   **URL**: `http://localhost:5000/api/Instructor/profile/details`
*   **Auth**: Bearer Token (Instructor)
*   **JSON Payload**:
    ```json
    {
      "fullName": "Jane Doe",
      "headline": "Lead Solutions Architect",
      "biography": "Ex-Netflix tech lead teaching .NET development",
      "profilePictureUrl": "/uploads/profile/janedoe.jpg"
    }
    ```

### 19. Instructor Request Payout
*   **Method**: `POST`
*   **URL**: `http://localhost:5000/api/Payouts/request`
*   **Auth**: Bearer Token (Instructor)
*   **JSON Payload**:
    ```json
    {
      "amount": 1500.00,
      "bankAccountNumber": "123456789012",
      "ifscCode": "SBIN0001234"
    }
    ```

### 20. View Payout History
*   **Method**: `GET`
*   **URL**: `http://localhost:5000/api/Payouts/history`
*   **Auth**: Bearer Token (Instructor)
