import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router'; 
import { environment } from '../../environments/environment';

declare var bootstrap: any;

@Component({
  selector: 'app-confirm-email', 
  templateUrl: './confirm-email.component.html', 
  styleUrls: ['./confirm-email.component.css'] 
}) 
export class ConfirmEmailComponent implements OnInit {
  private baseUrl = environment.apiUrl;
  
  token: string = '';
  userId: string = '';
  
  constructor(private route: ActivatedRoute, public http: HttpClient, public router: Router) {}
  
  ngOnInit(): void {
    // Capturando os valores dos parâmetros da URL
    this.userId = this.route.snapshot.queryParamMap.get('userId') || '';
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
    
    console.log('UserId:', this.userId, 'Token:', this.token); // Para depuração
  }
  
  errorMessage: string = ''; // Para armazenar mensagens de erro
  
  confirmemail() {
    // Certifique-se de que os parâmetros foram capturados corretamente
    if (!this.userId || !this.token) {
      this.errorMessage = 'Parâmetros inválidos. Não foi possível confirmar o email.';
      console.error(this.errorMessage);
	  
	  // Exibir o modal usando Bootstrap
	  const modalElement = document.getElementById('modal-notification');
	  const modalInstance = new bootstrap.Modal(modalElement);
	  modalInstance.show();
	  
      return;
    } 
    
    this.http.get(`${this.baseUrl}confirm-email?userId=${this.userId}&token=${this.token}`)
      .subscribe(
        () => {         		  
          
		  this.router.navigate(['/login']);
        },
        error => {
		  const apiMessage = error.error?.message || 'Erro não especificado pela API';
		
          this.errorMessage = `Erro ao confirmar email: ${apiMessage}`;
          
          // Exibir o modal usando Bootstrap
		  const modalElement = document.getElementById('modal-notification');
		  const modalInstance = new bootstrap.Modal(modalElement);
		  modalInstance.show();
        }
      );
  }
}