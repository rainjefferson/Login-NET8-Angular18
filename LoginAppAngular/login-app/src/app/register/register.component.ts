import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

declare var bootstrap: any;

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrl: './register.component.css'
})
export class RegisterComponent {
    private baseUrl = environment.apiUrl;
	
    email: string = '';
    password: string = '';
    confirmPassword: string = '';
    name: string = '';
    address: string = '';
    city: string = '';
    state: string = '';
    phone: string = '';
    
	errorMessage: string = '';
	
    constructor(private http: HttpClient, 
	            public router: Router) {}

    ngOnInit(): void {
      this.router.events.subscribe(() => {
        // Aqui vai o que vc precisar executar na inicialização
      });
    }

    register() {
        if (this.password !== this.confirmPassword) {
            this.errorMessage = "As senhas não correspondem!";
			
			// Exibir o modal usando Bootstrap
			const modalElement = document.getElementById('modal-notification');
			const modalInstance = new bootstrap.Modal(modalElement);
			modalInstance.show();
					
            return;
        }
		
		if (!this.name || !this.email || !this.password) {
            this.errorMessage = "Por favor, preencha todos os campos.";
			
			// Exibir o modal usando Bootstrap
			const modalElement = document.getElementById('modal-notification');
			const modalInstance = new bootstrap.Modal(modalElement);
			modalInstance.show();
					
            return;
        }
        
        this.http.post(`${this.baseUrl}register`, {
            email: this.email,
            password: this.password,
			confirmPassword: this.confirmPassword,
            name: this.name,
            address: this.address,
            city: this.city,
            state: this.state,
            phone: this.phone
        }).subscribe(() => {
            this.router.navigate(['/login']);
        },(error) => {                     
					
                    // Verifica se a API retornou uma mensagem
					const apiMessage = error.error?.message || 'Erro não especificado pela API';

					if (error.status === 401) {
					this.errorMessage = `Não autorizado. Detalhes: ${apiMessage}`;
					} else if (error.status === 500) {
					this.errorMessage = `Erro interno do servidor. Detalhes: ${apiMessage}`;
					} else {
					this.errorMessage = `Ocorreu um erro inesperado. Detalhes: ${apiMessage}`;
					}

					// Exibir o modal usando Bootstrap
					const modalElement = document.getElementById('modal-notification');
					const modalInstance = new bootstrap.Modal(modalElement);
			        modalInstance.show();
        });
    }
	
	onRegisterClick(event: Event): void {
		event.preventDefault(); // Evita o comportamento padrão do link
				
		this.router.navigate(['/login']).then(() => {
			setTimeout(() => {
				window.location.reload();
			}, 100);
		});
    }
}
