export class CommonResult<T = any> {
    success : boolean;
    message : string;
    result : T;
}