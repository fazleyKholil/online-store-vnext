module.exports = {
    createOrder
};

function createOrder() {
    var request = {
            "products": [
              {
                "productId": "0acc81de-de63-45b5-864a-dd6637a75ca4",
                "quantity": 1
              }
            ],
            "isShippingExpress": true,
            "shippingDistance": 100 
    };

    return request;
}
