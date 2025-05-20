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

let countConflicts (allExams: Exam list) (assignments: Assignment list) =
    assignments
    |> List.sumBy (fun a1 ->
        let exam1 = allExams |> List.find (fun e -> e.Id = a1.ExamId)
        assignments
        |> List.filter (fun a2 -> 
            a1.TimeSlot = a2.TimeSlot && 
            a1.ExamId <> a2.ExamId &&
            hasStudentOverlap exam1 (allExams |> List.find (fun e -> e.Id = a2.ExamId))
        )
        |> List.length
    ) / 2

let generateRandomAssignment (exams: Exam list) (rooms: Room list) (slots: string list) =
    let rnd = System.Random()
    exams 
    |> List.map (fun exam ->
        let validRooms = rooms |> List.filter (fun r -> fitsInRoom exam r)
        let validSlots = slots |> List.filter (fun s -> isValidSlot exam s)
        
        if List.isEmpty validRooms then
            failwithf "No hay aulas válidas para el examen %d (necesita capacidad para %d estudiantes)" exam.Id exam.Students.Length
        elif List.isEmpty validSlots then
            failwithf "No hay horarios válidos para el examen %d" exam.Id
        else
            let room = validRooms.[rnd.Next(validRooms.Length)]
            let slot = validSlots.[rnd.Next(validSlots.Length)]
            { ExamId = exam.Id; RoomId = room.Id; TimeSlot = slot }
    )

let minConflicts (exams: Exam list) (rooms: Room list) (slots: string list) (maxIterations: int) =
    let rnd = System.Random()
    let allExams = exams
    
    try
        let mutable currentSolution = generateRandomAssignment exams rooms slots
        let mutable currentConflicts = countConflicts allExams currentSolution
        let mutable iterations = 0

        while iterations < maxIterations && currentConflicts > 0 do
            let conflictingAssignments =
                currentSolution
                |> List.filter (fun a1 ->
                    let exam1 = allExams |> List.find (fun e -> e.Id = a1.ExamId)
                    currentSolution
                    |> List.exists (fun a2 -> 
                        a1.TimeSlot = a2.TimeSlot && 
                        a1.ExamId <> a2.ExamId &&
                        hasStudentOverlap exam1 (allExams |> List.find (fun e -> e.Id = a2.ExamId))
                    )
                )

            if not (List.isEmpty conflictingAssignments) then
                let assignmentToFix = conflictingAssignments.[rnd.Next(conflictingAssignments.Length)]

                let possibleAssignments =
                    [ for room in rooms do
                        for slot in slots do
                            let exam = allExams |> List.find (fun e -> e.Id = assignmentToFix.ExamId)
                            if fitsInRoom exam room && isValidSlot exam slot then
                                let newAssignment = { assignmentToFix with RoomId = room.Id; TimeSlot = slot }
                                let tempSolution = 
                                    currentSolution 
                                    |> List.map (fun a -> if a.ExamId = assignmentToFix.ExamId then newAssignment else a)
                                yield (tempSolution, countConflicts allExams tempSolution)
                    ]

                if not (List.isEmpty possibleAssignments) then
                    let bestSolution, bestConflicts = possibleAssignments |> List.minBy snd
                    currentSolution <- bestSolution
                    currentConflicts <- bestConflicts

            iterations <- iterations + 1

        Some { Assignments = currentSolution; Conflicts = currentConflicts }
    with
    | ex -> 
        printfn $"Error durante la generación: {ex.Message}"
        None

let rec backtrack (exams: Exam list) (rooms: Room list) (slots: string list) (assignment: Assignment list) (allExams: Exam list) =
    match exams with
    | [] -> Some (List.rev assignment)
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
                        a.TimeSlot = slot &&
                        let assignedExam = allExams |> List.find (fun e -> e.Id = a.ExamId)
                        hasStudentOverlap currentExam assignedExam
                    )
                if not hasConflict then
                    let newAssignment = { ExamId = currentExam.Id; RoomId = room.Id; TimeSlot = slot }
                    match backtrack remainingExams rooms slots (newAssignment::assignment) allExams with
                    | Some solution -> Some solution
                    | None -> tryCandidates rest
                else
                    tryCandidates rest
        tryCandidates candidates
