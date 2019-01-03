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
}