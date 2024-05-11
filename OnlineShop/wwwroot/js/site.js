//                                   Admin page
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Функция для загрузки формы добавления товара
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

// Обработчик клика на элементе "Добавить продукт" 
    $(document).on("click", "#addProduct", function (e) {
        e.preventDefault();
        loadAddProductForm(); // Загрузка представления с карточками товаров в #mainContent
        
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