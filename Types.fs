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