import { computed, Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoginService {
    
    
    private tokenSignal = signal<string | null>(
        sessionStorage.getItem('token')
    );

    isLoggedIn = computed(() => this.tokenSignal()!=null);
    username = computed(() => {
        const token = this.tokenSignal();
        if (!token) return null;

        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.unique_name;
    });

    setToken(token: string) {
        sessionStorage.setItem('token', token);
        console.log(token);
        this.tokenSignal.set(token);
    }

    clearToken() {
        sessionStorage.removeItem('token');
        this.tokenSignal.set(null);
    }

    getToken() {
        const token = this.tokenSignal();
        if (token==null || this.isExpired(token)) {
            this.clearToken();
            return null;
        }
        return token;
    }

    isExpired(token:string){
         if (!token) return true;

        const payload = JSON.parse(atob(token.split('.')[1]));
        const now = Math.floor(Date.now() / 1000);

        return payload.exp < now;
    }

    async Register(username:string, email:string, password:string){
        const url="https://localhost:7140/api/auth/register";

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username: username,
                email: email,
                password: password
            })
        });
        const data = await response.json()
        this.setToken(data.token)
    }

    async Login(usernameOrEmail:string, password:string){
        const url="https://localhost:7140/api/auth/login";

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                usernameOrEmail: usernameOrEmail,
                password: password
            })
        });
        const data = await response.json()
        this.setToken(data.token)
    }
}