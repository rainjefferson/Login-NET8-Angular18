import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

declare var bootstrap: any;

@Component({
    selector: 'app-forgot-password',
    templateUrl: './forgot-password.component.html',
    styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
    private baseUrl = environment.apiUrl;
	
    email: string = '';
    errorMessage: string = '';
	
    constructor(public http: HttpClient, public router: Router) {}

    resetPassword() {
	    this.errorMessage = ''; 
        
		if (!this.email) {
          this.errorMessage = 'Por favor, preencha o e-mail.';
		 
		  // Exibir o modal usando Bootstrap
		  const modalElement = document.getElementById('modal-notification');
		  const modalInstance = new bootstrap.Modal(modalElement);
		  modalInstance.show();
		 
          return; 
        }
	    
        this.http.post(`${this.baseUrl}forgot-password`, { email: this.email })
            .subscribe(
			    (response: any) => {
                this.errorMessage = 'A senha foi enviada para seu e-mail.';
                // Exibir o modal usando Bootstrap
				const modalElement = document.getElementById('modal-notification');
				const modalInstance = new bootstrap.Modal(modalElement);
				modalInstance.show();
				
				//this.router.navigate(['/login']);
                },
				(error) => {    
                    // Verifica se a API retornou uma mensagem
					const apiMessage = error.error?.message || 'Erro não especificado pela API';

					if (error.status === 401) {
					this.errorMessage = `Não autorizado. ${apiMessage}`;
					} else if (error.status === 500) {
					this.errorMessage = `Erro interno do servidor. ${apiMessage}`;
					} else {
					this.errorMessage = `Ocorreu um erro inesperado. ${apiMessage}`;
					}

					// Exibir o modal usando Bootstrap
					const modalElement = document.getElementById('modal-notification');
					const modalInstance = new bootstrap.Modal(modalElement);
			        modalInstance.show();
                }
		);
    }
	
	
	onRegisterClick(event: Event): void {
		event.preventDefault(); // Evita o comportamento padrão do link
				
		this.router.navigate(['/register']).then(() => {
			setTimeout(() => {
				window.location.reload();
			}, 100);
		});
    }
}

