import { Component, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { Country } from './../countries/Country';

@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.css']
})
export class CountryEditComponent {

  //titulo da View
  title: string;

  //a model do form
  form: FormGroup;

  //O objeto city para criação e edição
  country: Country;

  //o Id do objeto city, disponibilizado para ativar a rota:
  //Será NULL quando adicionarmos um novo país e NÃO será NULL quando editarmos um país existente
  id?: number;

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
    this.loadData();
  }

  ngOnInit() {
    this.form = this.fb.group({
      name: ['',
        Validators.required,
        this.isDupeField("name")
      ],
      iso2: ['',
        [
          Validators.required,
          Validators.pattern('[a-zA-Z]{2}')
        ],
        this.isDupeField("iso2")
      ],
      iso3: ['',
        [
          Validators.required,
          Validators.pattern('[a-zA-Z]{3}')
        ],
        this.isDupeField("iso3")
      ]
    });

    this.loadData();
  }

  loadData() {
    //recupera o ID do 'id'
    this.id = +this.activatedRoute.snapshot.paramMap.get('id');
    if (this.id) {
      // Modo edição, buscar cidade no servidor
      var url = this.baseUrl + "api/countries/" + this.id;

      this.http.get<Country>(url).subscribe(result => {
        this.country = result;
        this.title = "Edição - " + this.country.name;

        //atualização do form com os valores do país
        this.form.patchValue(this.country);
      }, error => console.error(error));
    }
    else {
      //Adição de pais
      this.title = "Criar um novo país";
    }
  }

  onSubmit() {
    var country = (this.id) ? this.country : <Country>{};

    country.name = this.form.get("name").value;
    country.iso2 = this.form.get("iso2").value;
    country.iso3 = this.form.get("iso3").value;

    if (this.id) {
      //Modulo Edição
      var url = this.baseUrl + "api/countries/" + this.country.id;

      this.http
        .put<Country>(url, country)
        .subscribe(result => {
          console.log("País " + country.name + "foi atualizado.");

          //volta para view de paises
          this.router.navigate(['/countries']);
        }, error => console.log(error));
    }
    else {
      //Módulo da adição
      var url = this.baseUrl + "api/countries";
      this.http
        .post<Country>(url, country)
        .subscribe(result => {
          console.log("País " + result.name + "foi criado");

          //volta para view de paises
          this.router.navigate(['/countries']);
        }, error => console.log(error));
    }
  }

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);

      var url = this.baseUrl + "api/countries/isDupeField";
      return this.http.post<boolean>(url, null, { params })
        .pipe(map(result => {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }
}
