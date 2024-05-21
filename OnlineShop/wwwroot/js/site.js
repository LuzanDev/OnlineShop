





















//                                   Admin page
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 //Функция для загрузки формы добавления товара
    function loadAddProductForm() 
    {
        $.ajax({
            url: "/Product/AddProduct", 
            type: "GET",
            success: function (data) {
                $("#mainContent").html(data); // Вставка результата AJAX-запроса в элемент #mainContent
            },
            error: function (error) {
                console.log(error);
            }
        });
}

function loadTableBrands() {
    $.ajax({
        url: "/Brand/Brands",
        type: "GET",
        success: function (data) {
            $("#mainContent").html(data); // Вставка результата AJAX-запроса в элемент #mainContent
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function loadTableCategories() {
    $.ajax({
        url: "/Category/Categories",
        type: "GET",
        success: function (data) {
            $("#mainContent").html(data); // Вставка результата AJAX-запроса в элемент #mainContent
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function loadTableProducts() {
    $.ajax({
        url: "/Product/Products",
        type: "GET",
        success: function (data) {
            $("#mainContent").html(data); // Вставка результата AJAX-запроса в элемент #mainContent
        },
        error: function (error) {
            console.log(error);
        }
    });
}



 //Обработчик клика на элементе "Добавить продукт"
    $(document).on("click", "#addProduct", function (e) {
        e.preventDefault();
        loadAddProductForm(); // Загрузка представления с карточками товаров в #mainContent

    });
$(document).on("click", "#loadProductsTable", function (e) {
    e.preventDefault();
    loadTableProducts();

});

$(document).on("click", "#loadBrandsTable", function (e) {
    e.preventDefault();
    loadTableBrands();

});

$(document).on("click", "#loadCategoriesTable", function (e) {
    e.preventDefault();
    loadTableCategories();

});


//Общие
// Функция для отображения модального окна с заданным заголовком и содержимым
function showModal(title, content) {
    // Очищаем футер модального окна перед добавлением новых кнопок
    var modalFooter = document.querySelector('#exampleModal .modal-footer');
    modalFooter.innerHTML = '';

    $('#exampleModal .modal-title').text(title); 
    $('#exampleModal .modal-body').html(content);
    $('#exampleModal').modal('show'); 
}
function resetModal() {
    // Находим модальное окно
    var modal = document.getElementById('exampleModal');

    // Находим диалоговое окно модального окна и удаляем класс для центрирования
    var modalDialog = modal.querySelector('.modal-dialog');
    modalDialog.classList.remove('modal-dialog-centered');

    // Сброс класса modal-lg
    modalDialog.classList.remove('modal-lg');

    // Находим заголовок модального окна
    var modalTitle = modal.querySelector('.modal-title');

    // Находим тело модального окна
    var modalBody = modal.querySelector('.modal-body');

    // Находим футер модального окна
    var modalFooter = modal.querySelector('.modal-footer');

    // Устанавливаем пустой заголовок
    modalTitle.textContent = '';

    // Очищаем тело модального окна
    modalBody.innerHTML = '';

    // Очищаем футер модального окна
    modalFooter.innerHTML = '';
}


// Функция для отображения уведомления успешного действия
function showSuccessNotification(message) {
    var notification = `
            <div class="alert alert-success d-flex align-items-center" role="alert">
                <svg class="bi flex-shrink-0 me-2" width="24" height="24" role="img" aria-label="Success:"><use xlink:href="#check-circle-fill" /></svg>
                <div>${message}</div>
            </div>
        `;

    // Удаляем все существующие уведомления перед добавлением нового
    $('#notification-container').empty();
    // Добавляем уведомление в контейнер
    $('#notification-container').append(notification);
    // Анимация появления
    $('#notification-container').fadeIn();
    // Закрываем уведомление через 3 секунды
    setTimeout(function () {
        $('#notification-container').fadeOut();
    }, 3000);
}
function showDeleteConfirmationModal(id, deletedItemName, deleteFunction) {
    resetModal();
    var modalTitle = document.querySelector('#exampleModal .modal-title');
    var modalBody = document.querySelector('#exampleModal .modal-body');

    modalTitle.textContent = 'Подтверждение удаления';
    modalBody.innerHTML = "Вы действительно хотите удалить <strong>" + deletedItemName + "</strong>?";

    // Создаем кнопки "Отмена" и "Да"
    var cancelButton = document.createElement('button');
    cancelButton.setAttribute('type', 'button');
    cancelButton.setAttribute('class', 'btn btn-secondary');
    cancelButton.setAttribute('data-bs-dismiss', 'modal');
    cancelButton.textContent = 'Отмена';

    var confirmButton = document.createElement('button');
    confirmButton.setAttribute('type', 'button');
    confirmButton.setAttribute('class', 'btn btn-danger');
    confirmButton.textContent = 'Да';

    // Очищаем футер модального окна перед добавлением новых кнопок
    var modalFooter = document.querySelector('#exampleModal .modal-footer');
    modalFooter.innerHTML = '';

    // Добавляем кнопки в футер модального окна
    modalFooter.appendChild(cancelButton);
    modalFooter.appendChild(confirmButton);

    // Отображаем модальное окно
    var modal = new bootstrap.Modal(document.getElementById('exampleModal'));
    modal.show();

    // Назначаем обработчик события для кнопки "Да"
    confirmButton.onclick = function () {
        deleteFunction(id); // Вызываем переданную функцию
        modal.hide();
    };
}
