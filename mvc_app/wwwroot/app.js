const API_URL = "/api/students";
let bootstrapModals = {};

document.addEventListener("DOMContentLoaded", () => {
    bootstrapModals['createModal'] = new bootstrap.Modal(document.getElementById('createModal'));
    bootstrapModals['editModal'] = new bootstrap.Modal(document.getElementById('editModal'));

    loadStudents();
});

function openModal(modalId) {
    if (modalId === 'createModal') document.getElementById("createForm").reset();
    bootstrapModals[modalId].show();
}
function closeModal(modalId) {
    bootstrapModals[modalId].hide();
}

async function loadStudents() {
    try {
        let response = await fetch(API_URL);

        if (response.status === 401) {
            window.location.href = "/login.html";
            return;
        }

        if (!response.ok) throw new Error("Ошибка сервера");

        let students = await response.json();
        let tbody = document.getElementById("studentsTableBody");
        tbody.innerHTML = "";

        if (students.length === 0) {
            tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">База данных пуста. Добавьте первого студента!</td></tr>`;
            return;
        }

        students.forEach(student => {
            tbody.innerHTML += `
                <tr>
                    <td>${student.id}</td>
                    <td>${student.name}</td>
                    <td>${student.email}</td>
                    <td>
                        <button class="btn btn-warning btn-sm fw-bold me-1" onclick="openEditForm(${student.id})">Изменить</button>
                        <button class="btn btn-danger btn-sm fw-bold" onclick="deleteStudent(${student.id})">Удалить</button>
                    </td>
                </tr>
            `;
        });
    } catch (error) {
        console.error("Не удалось загрузить студентов:", error);
        document.getElementById("studentsTableBody").innerHTML =
            `<tr><td colspan="4" class="text-center text-danger">Ошибка подключения к API бэкенда!</td></tr>`;
    }
}

async function saveNewStudent(event) {
    event.preventDefault();

    let studentData = {
        name: document.getElementById("createName").value,
        email: document.getElementById("createEmail").value,
        password: document.getElementById("createPassword").value
    };

    try {
        let response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(studentData)
        });

        if (response.ok) {
            closeModal('createModal');
            loadStudents();
        } else {
            alert("Ошибка при создании студента. Возможно, некорректные данные.");
        }
    } catch (error) {
        console.error("Ошибка запроса:", error);
    }
}

async function openEditForm(id) {
    try {
        let response = await fetch(`${API_URL}/${id}`);
        if (!response.ok) return alert("Не удалось получить данные студента");

        let student = await response.json();

        document.getElementById("editId").value = student.id;
        document.getElementById("editName").value = student.name;
        document.getElementById("editEmail").value = student.email;
        document.getElementById("editPassword").value = student.password;

        openModal('editModal');
    } catch (error) {
        console.error(error);
    }
}

async function saveStudentUpdate(event) {
    event.preventDefault();
    let id = document.getElementById("editId").value;

    let updatedData = {
        id: parseInt(id),
        name: document.getElementById("editName").value,
        email: document.getElementById("editEmail").value,
        password: document.getElementById("editPassword").value
    };

    try {
        let response = await fetch(`${API_URL}/${id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(updatedData)
        });

        if (response.ok) {
            closeModal('editModal');
            loadStudents();
        } else {
            alert("Не удалось обновить данные.");
        }
    } catch (error) {
        console.error(error);
    }
}

async function deleteStudent(id) {
    if (!confirm("Вы действительно хотите удалить этого студента?")) return;

    try {
        let response = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
        if (response.ok) {
            loadStudents();
        } else {
            alert("Ошибка при удалении.");
        }
    } catch (error) {
        console.error(error);
    }
}

function logout() {
    window.location.href = "/api/auth/logout";
}