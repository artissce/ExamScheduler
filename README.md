# 📅 Exam Schedule Generator (CSP in F#)

A Constraint Satisfaction Problem solver for scheduling exams without conflicts.

## 🧩 Key Features
- **Automatic assignment** of exams to rooms/time slots
- **Customizable constraints**:
  - 👥 Prevents students from having concurrent exams
  - 🏫 Respects room capacities
  - ⏰ Honors preferred time slots
- **Efficient algorithm** with backtracking and early pruning

## 🛠️ Quick Setup

1. Install [.NET 6+](https://dotnet.microsoft.com/download)
2. Clone repository
3. Run:
dotnet run 

## 🗂️ Code Structure
```
/ExamScheduler
├── Program.fs        # Usage example
├── Solver.fs         # Core CSP algorithm
├── Types.fs          # Data models
└── ExamScheduler.fsproj
```


## 📥 Input Example
```
let exams = [
    { Id = 1; Students = [1; 2; 3]; Duration = 2; RequiredSlots = ["Monday-9am"] }
    { Id = 2; Students = [3; 4]; Duration = 1; RequiredSlots = [] }
]

let rooms = [
    { Id = "RoomA"; Capacity = 50 }
    { Id = "RoomB"; Capacity = 2 }
]

let timeSlots = ["Monday-9am"; "Monday-11am"; "Tuesday-9am"]
```

## 📤 Output Example
```
✔ Schedule generated successfully!
Exam 1 (Students: [1; 2; 3]), Room: RoomA, Time: Monday-9am
Exam 2 (Students: [3; 4]), Room: RoomB, Time: Monday-11am
```

## 📄 License
MIT License

Made with ❤️ using F#. Questions? Open an issue!
