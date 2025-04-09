        $(document).ready(function () {
            $('.update-subtask-form').on('submit', function (e) {
                e.preventDefault();

                const $form = $(this);
                const formData = $form.serialize();

                $.post('/SubTask/UpdateStatus', formData)
                    .done(() => {
                        alert('✅ Subtask status updated!');
                    })
                    .fail(() => {
                        alert('❌ Failed to update subtask.');
                    });
            });
        });

let subtaskIndex = 1;

function addSubtask() {
    const container = document.getElementById('subtask-list');
    const newIndex = container.children.length; // Use current count as index

    const newInputGroup = document.createElement('div');
    newInputGroup.classList.add('input-group', 'mb-2');

    newInputGroup.innerHTML = `
        <input name="SubTasks[${newIndex}].Title" class="form-control" placeholder="Subtask Title" />
        <input type="hidden" name="SubTasks[${newIndex}].Status" value="Pending" />
        <button type="button" class="btn btn-outline-danger" onclick="this.parentElement.remove()">×</button>
    `;

    container.appendChild(newInputGroup);
}
