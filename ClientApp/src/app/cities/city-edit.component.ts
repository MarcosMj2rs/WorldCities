import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';

import { City } from './City';

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

  // objeto City para edição
  city: City;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      lat: new FormControl(''),
      lon: new FormControl('')
    });

    this.loadData();
  }

  loadData() {
    // recupera o ID a partir do parametro id
    var id = this.activatedRoute.snapshot.paramMap.get('id');

    // buscar a cidade no servidor
    var url = this.baseUrl + "api/cities/" + id;
    this.http.get<City>(url).subscribe(result => {
      this.city = result;
      this.title = "Edição - " + this.city.name;

      // atualizar o form com o valor da cidade
      this.form.patchValue(this.city);
    }, error => console.error(error));
  }

  onSubmit() {
    var city = this.city;

    city.name = this.form.get("name").value;
    city.lat = this.form.get("lat").value;
    city.lon = this.form.get("lon").value;

    console.log(city.name, city.lat, city.lon);

    var url = this.baseUrl + "api/cities/" + this.city.id;
    this.http
      .put<City>(url, city)
      .subscribe(resutl => {
        console.log("Cidade Id: [" + city.id + "] foi salvo.")

        // voltar para a visualização das cidades
        this.router.navigate(['/cities']);
      }, error => console.log(error));
  }
}
