import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  template: ''
})

export class BaseFormComponent {

  // form model
  form: FormGroup;

  constructor() { }

  // recuperar o FormControl
  getControl(name: string) {
    return this.form.get(name);
  }

  // retorna true se o formControl for valido
  isValid(name: string) {
    var e = this.getControl(name);
    return e && e.valid;
  }

  // retorna true se o formControl for modificado
  isChanged(name: string) {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched);
  }

  // retorna TRUE se o FormControl está gerando um erro, ou seja, um estado inválido após a alteração do usuário
  hasError(name: string) {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched) && e.invalid;
  }
}
