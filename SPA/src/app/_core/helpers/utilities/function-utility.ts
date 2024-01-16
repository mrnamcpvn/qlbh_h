import { Injectable } from "@angular/core";

@Injectable({ providedIn: "root" })
export class FunctionUtility {
  constructor() {}

  getNewUTCDate(date?: Date) {
    const d = date ? date : new Date();
    return new Date(
      Date.UTC(
        d.getFullYear(),
        d.getMonth(),
        d.getDate(),
        d.getHours(),
        d.getMinutes(),
        d.getSeconds(),
        d.getMilliseconds()
      )
    );
  }
  getStringDate(date?: Date) {
    date = date ?? new Date();
    return `${date.getFullYear()}/${date.getMonth() + 1 < 10 ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1)}/${date.getDate()} ${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;
  }
  getUTCDate(date?: Date) {
    date = date ?? new Date();
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds()));
  }
}
