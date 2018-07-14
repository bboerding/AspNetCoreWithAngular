class StoreCustomer {

    //Wenn die Parameter als private markiert sind, werden sie automatisch als private Member der Klasse instantiiert
    //D.h. man kann firstName und lastName direkt verwenden und muss sie nicht mehr explizit deklarieren
    constructor(private firstName: string, private lastName: string) {

    }

    public visits: number = 0;
    private ourName: string;

    public showName() {
        alert (this.firstName + " " + this.lastName);
    }


    get name() {
        return this.ourName;
    }
    set name(value) {
        this.ourName = value;
    }

}