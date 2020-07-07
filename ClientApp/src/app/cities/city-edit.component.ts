import { Component, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { City } from './City';
import { Country } from './../countries/Country';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.css']
})
export class CityEditComponent {
  // título da view
  title: string;

  // o modelo do formulário
  form: FormGroup;

  // objeto City para edição ou criação
  city: City;

  // o ID do objeto da city, tera o comportamento na rota ativa:
  // É NULL quando adicionamos uma nova cidade,
  // e NÃO NULL quando estamos editando uma existente.
  id?: number;

  // Um array de paises para selecção
  countries: Country[];

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', Validators.required),
      lon: new FormControl('', Validators.required),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());

    this.loadData();
  }

  loadData() {
    // Carregar países
    this.loadCountries();

    // recupera o ID a partir do parametro id
    this.id = +this.activatedRoute.snapshot.paramMap.get('id');

    if (this.id) {
      // Modo edição, buscar cidade no servidor
      var url = this.baseUrl + "api/cities/" + this.id;
      this.http.get<City>(url).subscribe(result => {
        this.city = result;
        this.title = "Edição - " + this.city.name;

        // atualizar o form com o valor da cidade
        this.form.patchValue(this.city);
      }, error => console.error(error));
    }
    else {
      // Modo Adição
      this.title = "Criar nova Cidade";
    }
  }

  loadCountries() {
    // Recipera todos os países do servidor
    var url = this.baseUrl + "api/countries";
    var params = new HttpParams()
      .set("pageSize", "9999")
      .set("sortColumn", "name");

    this.http.get<any>(url, { params }).subscribe(result => {
      this.countries = result.data;
    }, error => console.error(error));
  }

  onSubmit() {

    var city = (this.id) ? this.city : <City>{};

    city.name = this.form.get("name").value;
    city.lat = +this.form.get("lat").value;
    city.lon = +this.form.get("lon").value;
    city.countryId = +this.form.get("countryId").value;

    if (this.id) {
      // EDIÇÃO
      var url = this.baseUrl + "api/cities/" + this.city.id;
      this.http
        .put<City>(url, city)
        .subscribe(result => {

          console.log("City " + city.id + " foi atualizado.");

          // voltar para view Cidades
          this.router.navigate(['/cities']);
        }, error => console.log(error));
    }
    else {
      // ADIÇÃO
      var url = this.baseUrl + "api/cities";
      this.http
        .post<City>(url, city)
        .subscribe(result => {

          console.log("Cidade " + result.id + " foi criada.");

          // voltar para view Cidades
          this.router.navigate(['/cities']);
        }, error => console.log(error));
    }
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {

      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.get("name").value;
      city.lat = +this.form.get("lat").value;
      city.lon = +this.form.get("lon").value;
      city.countryId = +this.form.get("countryId").value;

      var url = this.baseUrl + "api/cities/IsDupeCity";
      return this.http.post<boolean>(url, city).pipe(map(result => {
        return (result ? { isDupeCity: true } : null);
      }));
    }
  }
}
