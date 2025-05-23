open System
open Types
open Solver

let exams = [
    { Id = 1; Students = [1; 2; 3]; Duration = 2; RequiredSlots = ["Lunes-11am"] }
    { Id = 2; Students = [4; 5]; Duration = 1; RequiredSlots = [] }
    { Id = 3; Students = [6; 7; 8]; Duration = 2; RequiredSlots = ["Martes-9am"] }
]

let rooms = [
    { Id = "AulaA"; Capacity = 3 }
    { Id = "AulaB"; Capacity = 2 }
    { Id = "AulaC"; Capacity = 4 }
]

let timeSlots = ["Lunes-9am"; "Lunes-11am"; "Martes-9am"; "Martes-11am"]

let validateResources (exams: Exam list) (rooms: Room list) (slots: string list) =
    let errors = ResizeArray<ScheduleError>()
    
    // Validar capacidad de aulas
    exams |> List.iter (fun exam ->
        if not (rooms |> List.exists (fun room -> fitsInRoom exam room)) then
            errors.Add(NoRoomsAvailableForExam exam.Id)
    )
    
    // Validar slots requeridos
    exams |> List.iter (fun exam ->
        if not exam.RequiredSlots.IsEmpty then
            let hasValidSlot = exam.RequiredSlots |> List.exists (fun reqSlot -> slots |> List.contains reqSlot)
            if not hasValidSlot then
                errors.Add(NoValidSlotsAvailable exam.Id)
    )
    
    if errors.Count > 0 then Error (List.ofSeq errors) else Ok ()

let printErrors errors =
    printfn "Errores encontrados durante la validación:"
    errors |> List.iter (function
        | NoRoomsAvailableForExam examId ->
            printfn $"  - Examen {examId}: No hay aulas con capacidad suficiente"
        | NoValidSlotsAvailable examId ->
            printfn $"  - Examen {examId}: No hay horarios disponibles que cumplan los requisitos"
        | _ -> ()
    )

let maxIterations = 1000

// Validación inicial
match validateResources exams rooms timeSlots with
| Error errors ->
    printErrors errors
    printfn "\nNo se puede generar el horario debido a los errores anteriores."
| Ok _ ->
    printfn "Recursos validados correctamente. Generando horarios..."
    
    printfn "\n=== Intentando con Backtracking ==="
    match backtrack exams rooms timeSlots [] exams with
    | Some solution ->
        printfn "¡Solución encontrada!"
        solution |> List.iter (fun a ->
            let exam = exams |> List.find (fun e -> e.Id = a.ExamId)
            printfn $"  Examen {a.ExamId} (Estudiantes: {exam.Students.Length}) -> {a.RoomId} @ {a.TimeSlot}")
    | None ->
        printfn "No se encontró solución con backtracking."
    
    printfn "\n=== Intentando con Mínimos Conflictos ==="
    match minConflicts exams rooms timeSlots maxIterations with
    | Some solution when solution.Conflicts = 0 ->
        printfn "¡Solución perfecta encontrada!"
        solution.Assignments |> List.iter (fun a ->
            let exam = exams |> List.find (fun e -> e.Id = a.ExamId)
            printfn $"  Examen {a.ExamId} (Estudiantes: {exam.Students.Length}) -> {a.RoomId} @ {a.TimeSlot}")
    | Some solution ->
        printfn $"Solución con {solution.Conflicts} conflictos restantes:"
        solution.Assignments |> List.iter (fun a ->
            let exam = exams |> List.find (fun e -> e.Id = a.ExamId)
            printfn $"  Examen {a.ExamId} -> {a.RoomId} @ {a.TimeSlot}")
        
        // Mostrar conflictos
        printfn "\nConflictos detectados:"
        solution.Assignments |> List.iter (fun a1 ->
            let exam1 = exams |> List.find (fun e -> e.Id = a1.ExamId)
            solution.Assignments |> List.iter (fun a2 ->
                if a1.ExamId < a2.ExamId && a1.TimeSlot = a2.TimeSlot then
                    let exam2 = exams |> List.find (fun e -> e.Id = a2.ExamId)
                    if hasStudentOverlap exam1 exam2 then
                        printfn $"  - Exámenes {a1.ExamId} y {a2.ExamId} comparten estudiantes en {a1.TimeSlot}"
            )
        )
    | None ->
        printfn "El algoritmo no encontró ninguna solución válida."