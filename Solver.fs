module Solver

open Types

let hasStudentOverlap exam1 exam2 =
    Set.intersect (Set.ofList exam1.Students) (Set.ofList exam2.Students)
    |> Set.isEmpty
    |> not

let fitsInRoom (exam: Exam) (room: Room) =
    exam.Students.Length <= room.Capacity

let isValidSlot (exam: Exam) (slot: string) =
    exam.RequiredSlots.IsEmpty || exam.RequiredSlots |> List.contains slot

let rec backtrack (exams: Exam list) (rooms: Room list) (slots: string list) (assignment: Assignment list) (allExams: Exam list) =
    match exams with
    | [] -> Some (List.rev assignment) // Invertimos para mantener el orden correcto
    | currentExam::remainingExams ->
        let candidates = [
            for room in rooms do
                for slot in slots do
                    if fitsInRoom currentExam room && isValidSlot currentExam slot then
                        yield (room, slot)
        ]
        let rec tryCandidates candidates =
            match candidates with
            | [] -> None
            | (room, slot)::rest ->
                let hasConflict = 
                    assignment |> List.exists (fun a ->
                        if a.TimeSlot = slot then
                            let assignedExam = allExams |> List.find (fun e -> e.Id = a.ExamId)
                            hasStudentOverlap currentExam assignedExam
                        else false
                    )
                if not hasConflict then
                    let newAssignment = { ExamId = currentExam.Id; RoomId = room.Id; TimeSlot = slot }
                    match backtrack remainingExams rooms slots (newAssignment :: assignment) allExams with
                    | Some solution -> Some solution
                    | None -> tryCandidates rest
                else
                    tryCandidates rest
        tryCandidates candidates