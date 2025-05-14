module Solver

open Types

// Verifica si dos exámenes comparten estudiantes
let hasStudentOverlap exam1 exam2 =
    // Usamos conjuntos para verificar la intersección
    // entre los estudiantes de ambos exámenes
    Set.intersect (Set.ofList exam1.Students) (Set.ofList exam2.Students) 
    //Retorna true si hay superposición (no está vacía la intersección)
    |> Set.isEmpty
    |> not

// Verifica si un aula tiene capacidad suficiente
let fitsInRoom (exam: Exam) (room: Room) =
    exam.Students.Length <= room.Capacity //si la longitud de estudiantes es menor a la capacidad del aula
    // Retorna true si el aula tiene capacidad suficiente

// Verifica si un horario es válido para un examen
let isValidSlot (exam: Exam) (slot: string) =
    exam.RequiredSlots.IsEmpty || exam.RequiredSlots |> List.contains slot
    //Si no hay horarios requeridos (IsEmpty), cualquier horario es válido
    //Si hay horarios requeridos, el slot debe estar en la lista

// Backtracking para asignar horarios
let rec backtrack (exams: Exam list) (rooms: Room list) (slots: string list) (assignment: Assignment list) =
    match exams with
    | [] -> Some assignment // Si no quedan examenes por asignar, retorna la solucion encontrada
    | currentExam::remainingExams -> //divide los examenes en el actual y los restantes
        let candidates = [//Genera todas las combinaciones posibles
            for room in rooms do//Cumplan con la capacidad del aula
                for slot in slots do //Sean horarios válidos para el examen
                    if fitsInRoom currentExam room && isValidSlot currentExam slot then
                        yield (room, slot)
        ]
        //Función recursiva interna que prueba cada candidato
        let rec tryCandidates candidates = 
            match candidates with
            | [] -> None//fallo, no hay candidatos
            | (room, slot)::rest ->
                let hasConflict = 
                    assignment |> List.exists (fun a -> //Verifica si el horario (slot) ya está asignado a otro examen que comparte con el actual
                        a.TimeSlot = slot && 
                        hasStudentOverlap currentExam { Id = a.ExamId; Students = []; Duration = 0; RequiredSlots = [] } // Mock para buscar el examen
                    )
                if not hasConflict then
                //crea nueva asignacion
                    let newAssignment = { ExamId = currentExam.Id; RoomId = room.Id; TimeSlot = slot }
                    //llamda recursiva con examenes restantes y la nueva asignacion
                    match backtrack remainingExams rooms slots (newAssignment :: assignment) with
                    //si el camino lleva a solucion, lo retorna
                    | Some solution -> Some solution
                    //sino prueba el siguiente candidato
                    | None -> tryCandidates rest
                else //si hay conflicto, pasa al siguiente candidato directamente
                    tryCandidates rest
        tryCandidates candidates