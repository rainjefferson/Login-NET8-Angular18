import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

declare const Scrollbar: any;
declare var bootstrap: any;

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
    private baseUrl = environment.apiUrl;
	
    email: string = '';
    password: string = '';
    errorMessage: string = ''; 
    isLoading: boolean = false; 

    constructor(public http: HttpClient, 
	            public router: Router) {}

	ngOnInit(): void {
        const win = navigator.platform.indexOf('Win') > -1;
        const sidenavScrollbar = document.querySelector('#sidenav-scrollbar');
        
        this.router.events.subscribe(() => {            

            if (win && sidenavScrollbar) {
                Scrollbar.init(sidenavScrollbar, { damping: '0.5' });
            }
        }); 
    }

    login() {
        this.isLoading = true; // Inicia o carregamento
        this.errorMessage = ''; 
        
		if (!this.email || !this.password) {
          this.errorMessage = 'Por favor, preencha todos os campos.';
		 
		  // Exibir o modal usando Bootstrap
		  const modalElement = document.getElementById('modal-notification');
		  const modalInstance = new bootstrap.Modal(modalElement);
		  modalInstance.show();
		 
          return; 
        }
		
        this.http.post(`${this.baseUrl}login`, { email: this.email, password: this.password })
            .subscribe(
                (response: any) => {
                    this.isLoading = false; 
                    // Simulação de redirecionamento após login bem-sucedido
                    this.router.navigate(['/main-menu']);
                },
                (error) => {
                    this.isLoading = false; 
					
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
	
	onForgotPasswordClick(event: Event): void {
		event.preventDefault(); // Evita o comportamento padrão do link
				
		this.router.navigate(['/forgot-password']).then(() => {
			setTimeout(() => {
				window.location.reload();
			}, 100);
		});
    }
}
