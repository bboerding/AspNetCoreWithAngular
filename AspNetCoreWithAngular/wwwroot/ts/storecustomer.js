var StoreCustomer = /** @class */ (function () {
    //Wenn die Parameter als private markiert sind, werden sie automatisch als private Member der Klasse instantiiert
    //D.h. man kann firstName und lastName direkt verwenden und muss sie nicht mehr explizit deklarieren
    function StoreCustomer(firstName, lastName) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.visits = 0;
    }
    StoreCustomer.prototype.showName = function () {
        alert(this.firstName + " " + this.lastName);
    };
    Object.defineProperty(StoreCustomer.prototype, "name", {
        get: function () {
            return this.ourName;
        },
        set: function (value) {
            this.ourName = value;
        },
        enumerable: true,
        configurable: true
    });
    return StoreCustomer;
}());
//# sourceMappingURL=storecustomer.js.map