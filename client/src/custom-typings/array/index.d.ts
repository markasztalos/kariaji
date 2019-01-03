interface Array<T> {
    mapMany<R>(selector : (value: T, index : number, array: T[]) => R[]) : R[];
    distinct() : T[];
}