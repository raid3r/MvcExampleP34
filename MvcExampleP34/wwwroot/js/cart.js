
// btn.add-to-cart
//<div class="cart-header-button position-relative">
//<div class="cart-count position-absolute badge bg-danger" style="bottom: -5px; left: -10px;">1</div>


// Отримати кількість товарів у кошику
async function updateCartItemCount() {

    await fetch('/Cart/Count')
        .then(response => response.json())
        .then(({ count }) => {
            // data:  {count: 123}
            setCartProductsCount(count)

        }).catch(err => {
            console.error(err)
            setCartProductsCount(0)
        })
}

function setCartProductsCount(count) {
    const cartCountElement = document.querySelector('div.cart-count');
    if (cartCountElement) {
        if (count > 0) {
            cartCountElement.textContent = count > 9 ? "9+" : count;
        } else {
            cartCountElement.textContent = "";
        }
    }
}

async function addToCart(productId, quantity) {
    await fetch('/Cart/Add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ productId: Number(productId), quantity: Number(quantity) })
    })
        .then(response => response.json())
        .then(({ success }) => {
            return success;
        })
};



// Обробник події для кнопок "Додати до кошика"
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('button.add-to-cart').forEach(button => {
        button.addEventListener('click', async (event) => {

            const productId = button.closest(".product-item").getAttribute('data-product-id');
            const success = await addToCart(productId, 1);

            if (success) { } {
                await updateCartItemCount();
            }

            // TODO : Показати повідомлення про успіх/помилку
        })

    })

    document.querySelectorAll('button.cart-item-button').forEach(button => {

        if (button.classList.contains('item-incrise')) {
            button.addEventListener('click', async (event) => {
                const productId = button.closest(".cart-item").getAttribute('data-product-id');
                await incriseQuantity(productId, 1);
            })
        }

        if (button.classList.contains('item-decrise')) {
            button.addEventListener('click', async (event) => {
                const productId = button.closest(".cart-item").getAttribute('data-product-id');
                await decriseQuantity(productId, 1);
            })
        }

        if (button.classList.contains('item-remove')) {
            button.addEventListener('click', async (event) => {
                const productId = button.closest(".cart-item").getAttribute('data-product-id');
                await removeFromCart(productId);
            })
        }
    })
})




updateCartItemCount();


//<button style="min-width: 50px; min-height: 50px; font-size: 20px;" class="btn btn-outline-primary item-decrise">-</button>
//                    <b class="btn " style="min-width:50px; font-size: 25px;">@item.Quantity</b>
//                    <button style="min-width: 50px; min-height: 50px; font-size: 20px" class="btn btn-outline-primary item-incrise">+</button>
//                    <button style="min-width: 50px; min-height: 50px; font-size: 20px" class="btn btn-outline-danger item-remove">X</button>


async function incriseQuantity(productId, quantity) {
    await fetch('/Cart/Increase', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            productId: Number(productId),
            quantity: Number(quantity)
        })
    })
        .then(response => response.json())
        .then(({ success }) => {
            window.location.reload();
        }).catch(err => {
            // do nothing
        })


}

async function decriseQuantity(productId, quantity) {
    await fetch('/Cart/Decrease', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            productId: Number(productId),
            quantity: Number(quantity)
        })
    })
        .then(response => response.json())
        .then(({ success }) => {
            window.location.reload();
        }).catch(err => {
            // do nothing
        })
}

async function removeFromCart(productId) {
    await fetch('/Cart/Remove', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ productId: Number(productId) })
    })
        .then(response => response.json())
        .then(({ success }) => {
            window.location.reload();
        }).catch(err => {
            // do nothing
        })

}
