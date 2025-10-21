# 📄 Contract Monthly Claim System (CMCS)

## 🧭 Overview
The **Contract Monthly Claim System (CMCS)** is a web-based prototype application built using **.NET 9 MVC**.  
It streamlines the submission, approval, and tracking of monthly claims for lecturers and allows Programme Coordinators and Academic Managers to manage claim approvals efficiently.

The system supports:
- Claim submissions
- File uploads
- Claim status tracking
- Admin approval and rejection
- Error handling and validation
- Unit testing
- Proper version control practices

---

## ✨ Features

- ✅ **Lecturer Claim Submission** – Submit claims with hours worked, rates, and notes.
- 📂 **File Upload** – PDF, DOCX, XLSX, PNG, JPG (max 35 MB), stored in `wwwroot/uploads`.
- 🧾 **Admin Review View** – Coordinators and Managers can approve or reject claims.
- 📊 **Status Tracking** – Real-time updates for Pending, Approved, and Rejected states.
- 🛡️ **Error Handling** – Friendly user messages and validation for file types and sizes.
- 🧪 **Unit Testing** – Tests cover Claim model calculations and controller logic.
- 🌿 **Version Control** – Multiple descriptive commits following good Git practices.

---

## 🧰 Technology Stack
- **Framework:** .NET 9 MVC  
- **Database:** SQL Server LocalDB (EF Core Code First)  
- **Testing:** xUnit  
- **UI:** Bootstrap 5  
- **IDE:** Visual Studio 2022

---

## 📦 Prerequisites
- .NET SDK 9 or later  
- Visual Studio 2022  
- SQL Server LocalDB installed

---

## 🚀 Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/UsmarTayler/ProgPoeST10445063.git
   cd ProgPoeST10445063
