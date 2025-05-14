open System
open Types
open Solver

let exams = [
    { Id = 1; Students = [1; 2; 3]; Duration = 2; RequiredSlots = ["Lunes-11am"] }
    { Id = 2; Students = [1; 2; 3]; Duration = 2; RequiredSlots = [] }
    { Id = 3; Students = [1; 2; 3]; Duration = 2; RequiredSlots = [] }
]

let rooms = [
    { Id = "AulaA"; Capacity = 2 }
    { Id = "AulaB"; Capacity = 2 }
]

let timeSlots = ["Lunes-9am"; "Lunes-11am"; "Martes-9am"]

match backtrack exams rooms timeSlots [] exams with  // Nota: ahora pasamos 'exams' dos veces
| Some solution ->
    printfn "¡Horario generado con exito!"
    solution |> List.iter (fun a -> 
        let exam = exams |> List.find (fun e -> e.Id = a.ExamId)
        printfn $"Examen {a.ExamId} (Estudiantes: {exam.Students}), Aula: {a.RoomId}, Horario: {a.TimeSlot}")
| None ->
    printfn "No se pudo generar un horario valido :("