// Функция проверки авторизации
async function isAuthenticated() {
    return fetch('/Account/IsAuthenticated')
        .then(response => response.json())
        .then(data => data.isAuthenticated)
        .catch(error => {
            console.error('Ошибка при проверке авторизации:', error);
            return false;
        });
}
// Получение избранных товаров
async function GetFavoriteProducts() {
    const isAuth = await isAuthenticated();
    //получение товаров из бд для авторизованного пользователя
    if (isAuth) {
        fetch('/Favorites/GetFavorites', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorMessage => {
                        throw new Error(errorMessage || 'Произошла ошибка');
                    });
                }
                return response.json();
            })
            .then(result => {
                const data = result.data; // Достаем data из возвращаемого объекта
                if (data.length > 0) {
                    renderFavorites(data); // Отображаем товары, если список не пуст
                } else {
                    document.getElementById('favorites-container').innerHTML = '<p>Ваш список избранного пуст.</p>';
                }
            })
            .catch(error => {
                // Выводим текст ошибки пользователю
                console.error('Error:', error);
                document.getElementById('favorites-container').innerHTML = `<p>${error.message}</p>`;
            });
    }
    else {
        // Получение товаров из локал для неавторизованного пользователя
        const favorites = JSON.parse(localStorage.getItem('favorites')) || [];
        if (favorites.length > 0) {
            // Отправляем запрос на сервер для получения информации о товарах
            fetch('/product/get-products-by-listid', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(favorites) // Передаем список избранных товаров в запросе
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(errorMessage => {
                            throw new Error(errorMessage || 'Произошла ошибка');
                        });
                    }
                    // Если успешен, возвращаем JSON
                    return response.json();
                })
                .then(data => {
                    if (data.length > 0) {
                        renderFavorites(data); // Отображаем товары, если список не пуст
                    } else {
                        document.getElementById('favorites-container').innerHTML = '<p>Ваш список избранного пуст.</p>';
                    }
                })
                .catch(error => {
                    // Выводим текст ошибки пользователю
                    console.error('Error:', error);
                    document.getElementById('favorites-container').innerHTML = `<p>${error.message}</p>`;
                });
        }
        else {
            document.getElementById('favorites-container').innerHTML = '<p>Ваш список избранного пуст.</p>';
        }
    }
}
// Функция для отрисовки "Избранных" товаров на странице
function renderFavorites(products) {
    const favoritesContainer = document.getElementById('favorites-container');
    let html = '';

    products.forEach(product => {
        // Получаем первое и второе изображение, если они есть
        const firstImage = product.images && product.images.length > 0 ? product.images[0].data : '';
        const secondImage = product.images && product.images.length > 1 ? product.images[1].data : '';

        // Форматируем цену, используя toLocaleString без символа валюты
        const formattedPrice = product.price.toLocaleString('uk-UA', { minimumFractionDigits: 0 });

        html += `
            <div class="col-lg-3 col-md-6 col-sm-12 mb-4">
                <div class="card" data-product-id="${product.id}">
                    <a href="/product/${product.id}">
                        <img src="data:image/jpeg;base64,${firstImage}"
                             data-hover-src="${secondImage ? 'data:image/jpeg;base64,' + secondImage : ''}"
                             class="card-img-top current-product-image"
                             alt="Product Image">
                    </a>
                    <button class="remove-button" onclick="removeFavoriteProductFromFavotetiesPage(${product.id})">
                        <img src="/img/cross.png" alt="Remove" class="remove-icon">
                    </button>
                    <div class="card-body">
                        <h4 class="card-title">${product.brand.name}</h4>
                        <p class="card-text">${product.description}</p>
                        <p class="card-text">${formattedPrice} ₴</p>
                    </div>
                </div>
            </div>
        `;
    });

    favoritesContainer.innerHTML = html;

    // Обработчик событий для смены изображений при наведении
    document.querySelectorAll('.current-product-image').forEach(img => {
        // Сохраняем оригинальный URL изображения
        const originalSrc = img.getAttribute('src');

        img.addEventListener('mouseover', function () {
            const hoverSrc = this.getAttribute('data-hover-src');
            if (hoverSrc) {
                this.setAttribute('src', hoverSrc);
            }
        });

        img.addEventListener('mouseout', function () {
            this.setAttribute('src', originalSrc);
        });
    });
}
// Обновление счетчика кол-ва избранных товаров
async function updateFavoriteCount() {
    const isAuth = await isAuthenticated();
    if (isAuth) {
        fetch('/Favorites/GetFavoriteCount', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorMessage => {
                        throw new Error(errorMessage || 'Произошла ошибка');
                    });
                }
                return response.json();
            })
            .then(data => {
                document.getElementById('favorites-count').textContent = data.count;
                if (data.count < 1) {
                    const event = new CustomEvent('NoneFavoriteProduct');
                    document.dispatchEvent(event);
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }
    else {
        const favorites = JSON.parse(localStorage.getItem('favorites')) || [];
        document.getElementById('favorites-count').textContent = favorites.length;
    }

}
// Удаление избранного товара
async function removeFromFavorites(productId) {
    const isAuth = await isAuthenticated();
    if (isAuth) {
        return fetch('/Favorites/DeleteFavorites', {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(productId)
        })
            .then(response => response.json())
            .then(data => {
                if (!data.success) {
                    console.error('Ошибка удаления на сервере:', data.errorMessage);
                }
                updateFavoriteCount();
            })
            .catch(error => console.error('Ошибка запроса:', error));
    }
    else {
        let favorites = JSON.parse(localStorage.getItem('favorites')) || [];
        favorites = favorites.filter(id => id.toString() !== productId.toString());

        localStorage.setItem('favorites', JSON.stringify(favorites));
        updateFavoriteCount();
        if (favorites.length < 1) {
            document.getElementById('favorites-container').innerHTML = '<p>Ваш список избранного пуст.</p>';
        }
    }
}
// Добавление избранного товара
async function addToFavorites(productId) {
    const isAuth = await isAuthenticated();
    if (isAuth) {
        return fetch('/Favorites/AddFavorites', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(productId) // Отправляем productId в теле запроса
        })
            .then(response => response.json())
            .then(data => {
                if (!data.success) {
                    console.error('Ошибка добавления на сервере:', data.errorMessage);
                }
                updateFavoriteCount();
            })
            .catch(error => console.error('Ошибка запроса:', error));
    }
    else {
        let favorites = JSON.parse(localStorage.getItem('favorites')) || [];
        favorites.push(productId);

        localStorage.setItem('favorites', JSON.stringify(favorites));
    }
    updateFavoriteCount();
}
// Удаление карточек избранных товаров 
function removeFavoriteProductFromFavotetiesPage(productId) {
    removeFromFavorites(productId);
    // Удаляем карточку товара из DOM
    const productCard = document.querySelector(`.card[data-product-id="${productId}"]`);
    if (productCard) {
        productCard.parentElement.remove();
    }
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
        url: "/product/management",
        type: "GET",
        success: function (data) {
            $("#mainContent").html(data); // Вставка результата AJAX-запроса в элемент #mainContent
        },
        error: function (error) {
            console.log(error);
        }
    });
}




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
