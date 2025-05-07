const apiBase = "/api/todo";

async function loadTodos() {
    const status = document.getElementById("statusFilter").value;
    const res = await fetch(`${apiBase}?status=${status}`);
    const todos = await res.json();

    const list = document.getElementById("todoList");
    list.innerHTML = "";

    todos.forEach(todo => {
        const li = document.createElement("li");
        li.className = "list-group-item";
        li.innerHTML = `
            <div>
                <strong>${todo.title}</strong> - ${todo.status} [${todo.priority}]
                <br />
                <small>Due: ${todo.dueDate?.split("T")[0] || "N/A"}</small>
            </div>
            <div>
                <button class="btn btn-sm btn-primary me-1" onclick="editTodo('${todo.id}')">Edit</button>
                <button class="btn btn-sm btn-success me-1" onclick="markComplete('${todo.id}')">Complete</button>
                <button class="btn btn-sm btn-danger" onclick="deleteTodo('${todo.id}')">Delete</button>
            </div>
        `;
        list.appendChild(li);
    });
}

document.getElementById("statusFilter").addEventListener("change", loadTodos);

document.getElementById("todoForm").addEventListener("submit", async (e) => {
    e.preventDefault();
    const todo = {
        id: document.getElementById("todoId").value,
        title: document.getElementById("title").value,
        description: document.getElementById("description").value,
        status: document.getElementById("status").value,
        priority: document.getElementById("priority").value,
        dueDate: document.getElementById("dueDate").value
    };

    const method = todo.id ? "PUT" : "POST";
    const endpoint = todo.id ? `${apiBase}/${todo.id}` : apiBase;

    const res = await fetch(endpoint, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(todo)
    });

    if (res.ok) {
        bootstrap.Modal.getInstance(document.getElementById("todoModal")).hide();
        document.getElementById("todoForm").reset();
        loadTodos();
    } else {
        alert("Error saving todo");
    }
});

function openCreateForm() {
    document.getElementById("todoModalLabel").innerText = "Create Todo";
    document.getElementById("todoForm").reset();
    document.getElementById("todoId").value = "";
}

async function editTodo(id) {
    const res = await fetch(`${apiBase}/${id}`);
    const todo = await res.json();

    document.getElementById("todoModalLabel").innerText = "Edit Todo";
    document.getElementById("todoId").value = todo.id;
    document.getElementById("title").value = todo.title;
    document.getElementById("description").value = todo.description;
    document.getElementById("status").value = todo.status;
    document.getElementById("priority").value = todo.priority;
    document.getElementById("dueDate").value = todo.dueDate?.split("T")[0] || "";

    new bootstrap.Modal(document.getElementById("todoModal")).show();
}

async function deleteTodo(id) {
    if (confirm("Are you sure you want to delete this todo?")) {
        await fetch(`${apiBase}/${id}`, { method: "DELETE" });
        loadTodos();
    }
}

async function markComplete(id) {
    await fetch(`${apiBase}/${id}/complete`, { method: "PUT" });
    loadTodos();
}

// Initial load
loadTodos();
