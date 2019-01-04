export function extendArrayType() {
    Array.prototype.mapMany = function <T, R>(
        this: Array<T>,
        selector: (value: T, index: number, array: T[]) => R[]): R[] {

        return this.map(selector).reduce((x, y) => x.concat(y), []);
    };
    Array.prototype.distinct = function <T>(this: Array<T>): T[] {
        const arr = [];
        for (const item of this)
            if (!arr.includes(item))
                arr.push(item);
        return arr;
    };  
    Array.prototype.distinctBy = function <T, R>(this: Array<T>, selector: (value: T, index: number, array: T[]) => R): T[] {
        const arr = [];
        let index = 0;
        for (const item of this) {
            if (!arr.find(x => selector(x, index, this) === selector(item, index, this))) {
                arr.push(item);
            }
            index++;
        }
        return arr;
    }; 
}