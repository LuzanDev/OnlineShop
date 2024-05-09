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
        url: "/Brand/GetListBrands",
        type: "GET",
        success: function (data) {
            $("#mainContent").html(data); // Вставка результата AJAX-запроса в элемент #mainContent
        },
        error: function (error) {
            console.log(error);
        }
    });

    //$.ajax({
    //    url: "/Brand/GetAllBrands", // Замените "Controller" на имя вашего контроллера
    //    type: "GET",
    //    success: function (data) {
    //        // Очищаем таблицу перед заполнением новыми данными
    //        $('#brandsTable').empty();
    //        // Заполняем таблицу данными
    //        $.each(data, function (index, brand) {
    //            var row = '<tr><td>' + (index + 1) + '</td><td>' + brand.text + '</td></tr>';
    //            $('#brandsTable').append(row);
    //        });
    //    },
    //    error: function (error) {
    //        console.log(error);
    //    }
    //});
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

// AddProduct
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////







//////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Общие
// Функция для отображения модального окна с заданным заголовком и содержимым
function showModal(title, content) {
    $('#exampleModal .modal-title').text(title); 
    $('#exampleModal .modal-body').html(content);
    $('#exampleModal').modal('show'); 
}