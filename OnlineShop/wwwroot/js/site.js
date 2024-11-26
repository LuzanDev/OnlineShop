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
// Navigation ----------------------------
const categoryList = document.querySelector('.category-list');
const scrollLeftBtn = document.querySelector('.scroll-left');
const scrollRightBtn = document.querySelector('.scroll-right');
let scrollInterval;
function getScrollDistance() {
    if (window.innerWidth <= 576) { // Для маленьких экранов (например, до 576px)
        return 150;
    } else if (window.innerWidth <= 768) { // Для средних экранов
        return 250;
    } else { // Для больших экранов
        return 450;
    }
}
function scrollCategories(direction) {
    const step = 5; // Пиксели для плавного смещения
    const distance = getScrollDistance(); // Определяем нужное расстояние
    let scrolled = 0;

    clearInterval(scrollInterval); // Остановка предыдущего интервала, если он существует

    scrollInterval = setInterval(() => {
        categoryList.scrollBy({ left: direction * step });
        scrolled += step;

        if (scrolled >= distance) {
            clearInterval(scrollInterval); // Остановка, когда достигли целевого расстояния
        }
    }, 10); // Интервал в миллисекундах
}
function toggleArrows() {
    // Отображение стрелки "влево" при прокрутке вправо
    scrollLeftBtn.style.display = categoryList.scrollLeft > 0 ? 'block' : 'none';

    // Скрытие стрелки "вправо" при достижении конца списка
    const maxScrollLeft = categoryList.scrollWidth - categoryList.clientWidth;
    scrollRightBtn.style.display = categoryList.scrollLeft < maxScrollLeft ? 'block' : 'none';
}
// Navigation ----------------------------

//  "Корзина"
//Обработчик кнопки Оформления заказа
async function handleCheckoutClick() {
    const auth = await isAuthenticated();
    if (!auth) {
        const modal = new bootstrap.Modal(document.getElementById('authModal'));
        modal.show();
        return;
    }
    window.location.href = '/checkout';
}



// Рендер таблицы если товары есть в корзине
function renderCartContent(data) {
    const cartContent = document.getElementById("cartContent");

    if (data.cartItems.length > 0) {
        cartContent.innerHTML = `
        <div class="col-lg-9 col-12">
            <table class="table">
                <thead>
                    <tr>
                        <th class="text-left" style="width: 110px;"></th>
                        <th>Информация</th>
                        <th>Цена</th>
                        <th class="text-end">Количество</th>
                        <th class="text-end" style="width: 40px;"></th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
        <div class="col-lg-3 col-12">
            <div class="card mt-3 mt-lg-0">
                <div class="card-body">
                    <h5 class="card-title">Сумма заказа</h5>
                    <p id="totalAmount" class="card-text">3000₽</p>
                    <button id="btnCheckout" class="btn btn-dark btn-pay-order-cart">Оплатить</button>
                </div>
            </div>
        </div>
    `;
        document.getElementById('btnCheckout').addEventListener('click', handleCheckoutClick);
        renderCartItems(data.cartItems);
        updateTotal(data.totalAmount);
    }
    else {
        cartContent.innerHTML = '<p>Сейчас в корзине ничего нет.<br>Войдите или создайте аккаунт, чтобы вступить в программу лояльности, получить доступ к привилегиям и персональным рекомендациям.</p>';
    }

}
// Получение общей суммы корзины (Auth)
function UpdateTotalAmount() {
    fetch('/Cart/GetTotalAmount', {
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
            updateTotal(result.data);
        })
        .catch(error => {
            console.error('Error:', error);
        });
}
// Отображение товаров в корзине (Auth/NotAuth)
async function GetCart() {

    const isAuth = await isAuthenticated();
    if (isAuth) {
        fetch('/Cart/GetCart', {
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
                const data = result.data;
                renderCartContent(data);
                updateCartCount(true);
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }
    else {
        const cartItem = JSON.parse(localStorage.getItem('cart')) || [];

        fetch('/Cart/GetCartItemsNotAuthorized', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cartItem)
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
                const data = result.data;
                renderCartContent(data);
                updateCartCount(false);

            })
            .catch(error => {
                console.error('Error:', error);
            });
    }

}
// Отображение товаров которые находятся в корзине
function renderCartItems(cartItems) {
    const tbody = document.querySelector('.table tbody');
    tbody.innerHTML = ''; // Очищаем предыдущие строки

    cartItems.forEach(item => {
        const row = document.createElement('tr');
        const imageSrc = item.product.images && item.product.images.length > 0
            ? `/images/products/${item.product.images[0].fileName}`
            : '';

        row.innerHTML = `
            <td>
                <img src="${imageSrc}" alt="${item.product.name}">
            </td>
            <td style="vertical-align: middle;">
                ${item.product.name}<br>
                ${item.product.description}<br>
            </td>
            <td id="total-price-id-${item.productId}" style="vertical-align: middle;">
                ${item.total.toLocaleString('uk-UA')} ₴
            </td>
            <td style="vertical-align: middle;">
                <div class="d-flex justify-content-end align-items-center">
                    
                    
                    <div class="dropdown-container d-flex align-items-center"> 
                        <span id="quantityText_${item.productId}" class="cus-span">${item.quantity}</span>
                        <a class="btn btn-edit-count" id="editButton_${item.productId}" onclick="showDropdown(${item.productId})">Изменить</a>
                        <div class="dropdown" id="quantityDropdown_${item.productId}" style="display: none;">
                            <select class="form-control d-inline-block" style="width: auto;" id="quantitySelect_${item.productId}" onchange="updateQuantity(${item.productId})">
                                ${[...Array(9).keys()].map(i => `<option value="${i + 1}" ${i + 1 === item.quantity ? 'selected' : ''}>${i + 1}</option>`).join('')}
                            </select>
                            <a class="btn btn-edit-count" onclick="cancel(${item.productId})">Отмена</a>
                        </div>
                    </div>
                </div>
            </td>
            <td id="remove-product-id-${item.productId}" style="vertical-align: top;">
                <img src="/img/cross.png" alt="Remove" onclick="removeCartItemFromCartPage(${item.productId})">
            </td>
        `;

        tbody.appendChild(row);
    });

}
// Добавление товара в корзину
function addProductToCarts(productId) {
    return fetch('/Cart/AddProduct', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(productId)
    })
        .then(response => response.json())
        .then(data => {
            if (!data.success) {
                console.error('Ошибка добавления на сервере:', data.errorMessage);
            }
            updateCartCount(true);
        })
        .catch(error => console.error('Ошибка запроса:', error));
}
// Удаление товара из корзины
async function removeProductFromCarts(productId) {
    return fetch('/Cart/DeleteProduct', {
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
            updateCartCount(true);
            return data;
        })
        .catch(error => console.error('Ошибка запроса:', error));
}
// Удаление товара из корзины на странице корзины 
async function removeCartItemFromCartPage(productId) {

    const isAuth = await isAuthenticated();
    if (isAuth) {
        var response = await removeProductFromCarts(productId);
        const result = response.data;
        renderCartContent(result);
    }
    else {
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        cart = cart.filter(item => item.ProductId !== String(productId));
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartCount(false);

        fetch('/Cart/GetCartItemsNotAuthorized', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cart)
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
                const data = result.data;
                renderCartContent(data);
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }
}
// Синхронизация корзины
async function syncCartItems() {
    const cartItems = JSON.parse(localStorage.getItem('cart')) || [];
    const cartItemsId = cartItems.map(item => parseInt(item.ProductId));

    if (cartItemsId.length > 0) {
        try {
            const response = await fetch('/Cart/SyncCartItem', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(cartItemsId),
            });

            const data = await response.json();
            if (data.success) {
                console.log('Корзина успешко синхронизована');
                // Дополнительные действия, например, очистка localStorage
                // updateFavoriteButtons(favoriteProducts);

                localStorage.removeItem('cart');
            } else {
                console.error('Failed to sync favorites:', data.message || 'Unknown error');
            }
        } catch (error) {
            console.error('Error syncing favorites:', error);
        }
    }
    updateCartCount(true);
}
// Обновление счетчика кол-ва товаров в корзине
function updateCartCount(isAuth) {
    //const isAuth = await isAuthenticated();
    if (isAuth) {
        fetch('/Cart/GetCartItemCount', {
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
                document.getElementById('cart-count').textContent = data.count;
                if (data.count < 1) {
                    // const event = new CustomEvent('NoneCartItem');
                    // document.dispatchEvent(event);
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });
    }
    else {
        const cart = JSON.parse(localStorage.getItem('cart')) || [];
        document.getElementById('cart-count').textContent = cart.length;
    }

}

function showDropdown(itemId) {
    document.getElementById(`quantityText_${itemId}`).style.display = 'none';
    document.getElementById(`editButton_${itemId}`).style.display = 'none';
    document.getElementById(`quantityDropdown_${itemId}`).style.display = 'block';
}
async function updateQuantity(productId) {
    const selectedValue = document.getElementById(`quantitySelect_${productId}`).value;

    document.getElementById(`quantityDropdown_${productId}`).style.display = 'none';
    document.getElementById(`editButton_${productId}`).style.display = 'inline';
    document.getElementById(`quantityText_${productId}`).style.display = 'inline';

    const isAuth = await isAuthenticated();
    if (isAuth) {
        fetch('/Cart/UpdateItemQuantity', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                productId: productId,    // Идентификатор продукта
                quantity: selectedValue, // Новое количество
            })
        })
            .then(response => {
                if (response.ok) {
                    return response.json(); // Обрабатываем успешный ответ
                } else {
                    throw new Error('Failed to update quantity');
                }
            })
            .then(response => {

                const formattedValue = response.data.toLocaleString('uk-UA'); // Форматирование для Украины
                document.getElementById(`total-price-id-${productId}`).innerText = `${formattedValue} ₴`;
                UpdateTotalAmount(); // Oбщая сумма корзины (Auth)

            })
            .catch(error => {
                console.error('Error updating quantity:', error);
            });
    }
    else {
        const cartItem = JSON.parse(localStorage.getItem('cart')) || [];

        const oldCount = parseInt(document.getElementById(`quantityText_${productId}`).innerText, 10);

        let oldPrice = document.getElementById(`total-price-id-${productId}`).innerText;
        oldPrice = parseFloat(oldPrice.replace(/\s/g, '').replace('₴', ''));

        const pricePer1Piece = oldPrice / oldCount;
        const newPrice = pricePer1Piece * selectedValue;

        let generalValue = document.getElementById(`totalAmount`).innerText;
        generalValue = parseFloat(generalValue.replace('Товары:', '').replace(/\s/g, '').replace('₴', ''));
        const newGeneralValue = (generalValue - oldPrice) + newPrice;

        const updatedCart = cartItem.map(item => {
            if (item.ProductId === String(productId)) {
                return { ...item, Quantity: parseInt(selectedValue, 10) }; // Обновляем количество
            }
            return item; // Возвращаем остальные товары без изменений
        });

        // Сохраняем обновлённый объект в localStorage
        localStorage.setItem('cart', JSON.stringify(updatedCart));

        const formattedValue = newPrice.toLocaleString('uk-UA'); // Форматирование для Украины
        document.getElementById(`total-price-id-${productId}`).innerText = `${formattedValue} ₴`;
        updateTotal(newGeneralValue);
    }
    document.getElementById(`quantityText_${productId}`).innerText = selectedValue;

}
function cancel(itemId) {
    document.getElementById(`quantityDropdown_${itemId}`).style.display = 'none';
    document.getElementById(`editButton_${itemId}`).style.display = 'inline';
    document.getElementById(`quantityText_${itemId}`).style.display = 'inline';
}
// Изменение  суммы корзины JS
function updateTotal(totalAmount) {
    const totalElement = document.getElementById('totalAmount');
    totalElement.textContent = `Товары: ${totalAmount.toLocaleString('uk-UA')} ₴`;
}










////////  ИЗБРАННОЕ ////////

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
        const firstImage = product.images && product.images.length > 0 ? product.images[0].fileName : '';
        const secondImage = product.images && product.images.length > 1 ? product.images[1].fileName : '';

        // Форматируем цену, используя toLocaleString без символа валюты
        const formattedPrice = product.price.toLocaleString('uk-UA', { minimumFractionDigits: 0 });

        html += `
            <div class="col-lg-3 col-md-4 col-6 mb-4">
                <div class="card" data-product-id="${product.id}">
                    <a href="/product/${product.id}">
                        <img src="/images/products/${firstImage}"
                             data-hover-src="${secondImage ? '/images/products/' + secondImage : ''}"
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
async function updateFavoriteCount(isAuth) {
    //const isAuth = await isAuthenticated();
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
                updateFavoriteCount(true);
            })
            .catch(error => console.error('Ошибка запроса:', error));
    }
    else {
        let favorites = JSON.parse(localStorage.getItem('favorites')) || [];
        favorites = favorites.filter(id => id.toString() !== productId.toString());

        localStorage.setItem('favorites', JSON.stringify(favorites));
        updateFavoriteCount(false);
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
                updateFavoriteCount(true);
            })
            .catch(error => console.error('Ошибка запроса:', error));
    }
    else {
        let favorites = JSON.parse(localStorage.getItem('favorites')) || [];
        favorites.push(productId);

        localStorage.setItem('favorites', JSON.stringify(favorites));
        updateFavoriteCount(false);
    }

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
// Синхронизация избранных товаров
async function syncFavorites() {

    const favoriteProducts = JSON.parse(localStorage.getItem('favorites')) || [];

    if (favoriteProducts.length > 0) {
        try {
            const response = await fetch('/Favorites/SyncFavorites', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(favoriteProducts),
            });

            const data = await response.json();
            if (data.success) {
                console.log('Favorites synchronized successfully');
                // Дополнительные действия, например, очистка localStorage
                updateFavoriteButtons(favoriteProducts);
                updateFavoriteCount(true);
                localStorage.removeItem('favorites');
            } else {
                console.error('Failed to sync favorites:', data.message || 'Unknown error');
            }
        } catch (error) {
            console.error('Error syncing favorites:', error);
        }
    }

}
// Функция для обновления кнопок "избранное" после синхронизации
function updateFavoriteButtons(favoriteProductIds) {
    document.querySelectorAll('.card').forEach(card => {
        const productId = card.getAttribute('data-product-id');

        if (favoriteProductIds.includes(productId)) {
            const favoriteButton = card.querySelector('.favorite-button');
            favoriteButton.classList.add('active');
        }
    });

    const productFavoriteButton = document.querySelector('#product-favorite-btn');
    if (productFavoriteButton) {
        const productId = productFavoriteButton.getAttribute('data-product-id');

        if (favoriteProductIds.includes(productId)) {
            productFavoriteButton.classList.add('active');
        }
    }
}

// Переменные для сохранения данных модального окна
var originalModalBody;
var originalModalHeaderName;
// Сохранение данных модального окна authModal
function saveOriginalAuthModal() {
    var authModal = document.getElementById('authModal');

    originalModalBody = document.querySelector('#authModal .modal-body').innerHTML;
    originalModalHeaderName = document.querySelector('#authModal .modal-header-name').innerHTML;

    authModal.addEventListener('show.bs.modal', function () {
        resetModalContent();
    });
}
// Востановление AuthModal в исходные данные
function resetModalContent() {
    document.querySelector('#authModal .modal-body').innerHTML = originalModalBody;
    document.querySelector('#authModal .modal-header-name').innerHTML = originalModalHeaderName;

    var loginTab = new bootstrap.Tab(document.getElementById('login-tab'));
    loginTab.show();
}












// Функия выхода пользователя из аккаунта
function logoutUser() {
    $.ajax({
        url: '/Account/Logout',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                window.location.reload();
                console.log('Пользователь успешно вышел');
            }
        },
        error: function (xhr, status, error) {
            console.log('Произошла ошибка при попытке выхода');
            var errorMessage = xhr.responseJSON.errorMessage;
            window.location.href = '/Account/Error?errorMessage=' + encodeURIComponent(errorMessage);
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
