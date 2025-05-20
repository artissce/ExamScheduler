module Types

type Exam = {
    Id: int
    Students: int list
    Duration: int
    RequiredSlots: string list
}

type Room = {
    Id: string
    Capacity: int
}

type Assignment = {
    ExamId: int
    RoomId: string
    TimeSlot: string
}

type ScheduleError =
    | RoomCapacityExceeded of examId: int * roomId: string
    | NoValidSlotsAvailable of examId: int
    | StudentConflict of examId1: int * examId2: int * timeSlot: string
    | NoRoomsAvailableForExam of examId: int

type Solution = {
    Assignments: Assignment list
    Conflicts: int  // Número de conflictos en esta solución
}