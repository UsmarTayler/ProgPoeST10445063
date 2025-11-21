# CMCS – Contract Monthly Claim System  
### Final Project Submission (Part 3)  
**ASP.NET Core MVC · EF Core · SQL Server · Role-Based Access Control**
https://github.com/UsmarTayler/ProgPoeST10445063.git
---

## 📌 Overview

The **Contract Monthly Claim System (CMCS)** is a role-based web platform that streamlines the submission, approval, and processing of contract lecturer claims.

The system includes:

- Automated claim calculations  
- Secure role-based login  
- Coordinator & Manager approval flows  
- HR reporting + lecturer management  
- Monthly summaries + process workflows  
- File uploads with validation  

This README provides setup instructions, credentials, and feature coverage aligned with rubric requirements.

---

# 🔐 Login Credentials (Required for Marking)

### Lecturer  
- **Username:** lecturer1  
- **Password:** Pass123!

### Programme Coordinator  
- **Username:** coord1  
- **Password:** Pass123!

### Manager  
- **Username:** manager1  
- **Password:** Pass123!

### HR (Super User)  
- **Username:** hr1  
- **Password:** Pass123!

---

# 🚀 System Features (Rubric Aligned)

## 1️⃣ Lecturer Automation (20 Marks)

- Auto-load hourly rate when selecting a lecturer  
- Auto-calculate `TotalAmount = HoursWorked × HourlyRate`  
- File upload validation for safe file types  
- Claim status auto-set to **Pending**  
- Submission date saved automatically  
- Clean UI for claim creation  
- EF Core inserts claim + documents into database  

---

## 2️⃣ Coordinator & Manager Automation (20 Marks)

- Role-restricted access via custom `RequireRole` attribute  
- Approve / Reject workflow  
- Coordinators see **Pending** claims  
- Managers see escalated Pending claims  
- Approval updates status to:
  - **Approved (1)**  
  - **Rejected (2)**  
- Users cannot self-register — HR manages accounts  
- Navigation automatically changes based on role  

---

## 3️⃣ HR Automation & Reporting (20 Marks)

- HR can **create, edit, and delete** lecturers  
- Monthly summary grouped by lecturer  
- Summary includes:
  - Total hours  
  - Total amount  
  - Number of claims  
- HR can mark approved claims as **Processed (3)**  
- Separate HR dashboard and workflow  

---

## 4️⃣ PowerPoint Presentation (20 Marks)

- Clean and structured  
- Visual slides with screenshots  
- Covers Part 2 + Part 3 functionality  
- No code included (per rubric requirement)  

---

## 5️⃣ Design & User Friendliness (10 Marks)

- Bootstrap UI with consistent styling  
- Logical navigation for each role  
- Easy-to-use forms and tables  
- Clear labeling and UI feedback  

---

# 🗄️ Project Setup

## Requirements
- Visual Studio 2022  
- .NET 8 SDK  
- SQL Server / LocalDB  
- EF Core Tools  

---

## 1. Database Setup

Run the following in **Package Manager Console**:


This will create:

- Lecturers table  
- Claims table  
- Supporting documents table  
- Seeded admin & lecturer accounts  

---

## 2. Run the Application

1. Set **CMCS.Mvc** as the startup project  
2. Run using **F5**  
3. Log in with the credentials listed above  

---

# 🧪 Unit Tests

Unit tests are stored in:


---

# 🤖 AI Assistance Declaration

AI (ChatGPT) was used **only for**:

- Debugging  
- Error explanation  
- Suggesting improvements  
- Fixing controllers and views  
- Helping structure documentation  

**No full project components or assessment content were created solely by AI.**

---

# 👤 Developer

**Taysu**
