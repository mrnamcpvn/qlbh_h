import { Injectable } from '@angular/core';
interface Dictionary {
  units: string[];
  tens: string[];
  hundreds: string[];
}
@Injectable({
  providedIn: 'root'
})

export class ToVietnameseService {

constructor() { }
  private defaultNumbers = ' hai ba bốn năm sáu bảy tám chín';
  private dict: Dictionary = {
    units: ('? một' + this.defaultNumbers).split(' '),
    tens: ('lẻ mười' + this.defaultNumbers).split(' '),
    hundreds: ('không một' + this.defaultNumbers).split(' '),
  };

  private tram = 'trăm';
  private digits = 'x nghìn triệu tỉ nghìn'.split(' ');

  private tenth(blockOf2: string[]): string {
    let sl1 = this.dict.units[blockOf2[1]];
    const result = [this.dict.tens[blockOf2[0]]];
    if (blockOf2[0] > '0' && blockOf2[1] === '5') sl1 = 'lăm';
    if (blockOf2[0] > '1') {
      result.push('mươi');
      if (blockOf2[1] === '1') sl1 = 'mốt';
    }
    if (sl1 !== '?') result.push(sl1);
    return result.join(' ');
  }

  private blockOfThree(block: string): string {
    switch (block.length) {
      case 1:
        return this.dict.units[block];

      case 2:
        return this.tenth(block.split(''));

      case 3:
        const result = [this.dict.hundreds[block[0]], this.tram];
        if (block.slice(1, 3) !== '00') {
          const sl12 = this.tenth(block.slice(1, 3).split(''));
          result.push(sl12);
        }
        return result.join(' ');
    }
    return '';
  }

  private formatNumber(nStr: string): string {
    nStr += '';
    const x = nStr.split('.');
    let x1 = x[0];
    const x2 = x.length > 1 ? '.' + x[1] : '';
    const rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
      x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
  }

  private digitCounting(i: number, digitCounter: number): string {
    return this.digits[i];
  }

  toVietnamese(input: number, currency?: string): string {
    let str = input.toString();
    let index = str.length;
    if (index === 0 || str === 'NaN') return '';
    let i = 0;
    let arr: string[] = [];
    let result: string[] = [];

    // explode number string into blocks of 3 numbers and push to queue
    while (index >= 0) {
      arr.push(str.substring(index, Math.max(index - 3, 0)));
      index -= 3;
    }
    // loop through queue and convert each block
    let digitCounter = 0;
    let digit;
    for (i = arr.length - 1; i >= 0; i--) {
      if (arr[i] === '000') {
        digitCounter += 1;
        if (i === 2 && digitCounter === 2) {
          result.push(this.digitCounting(i + 1, digitCounter));
        }
      } else if (arr[i] !== '') {
        digitCounter = 0;
        result.push(this.blockOfThree(arr[i]));
        digit = this.digitCounting(i, digitCounter);
        if (digit && digit !== 'x') result.push(digit);
      }
    }
    if (currency) result.push(currency);
    // remove unwanted white space
    return result.join(' ');
  }
}


