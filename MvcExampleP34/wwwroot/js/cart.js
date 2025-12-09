
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

});




updateCartItemCount();