# Frontend Integration Notes

## ApiService / AuthService response shape

Backend returns `ApiResponse<T>` where payload sits in `.data`.
Components expect list results at `res.data.items` and single resources at `res.data`.

### ApiService helper (unwrap example)

You can add a small helper in `ApiService` to automatically unwrap `ApiResponse`:

```ts
get<T>(url: string, params: any = {}) {
  return this.http.get<ApiResponse<T>>(this.baseUrl + url, { params }).pipe(
    map(res => res.data as T)
  );
}
```

### Usage in components

If `ApiService` unwraps automatically:

```ts
this.api.get<PagedResponse<DoctorDto>>('/api/doctors', filters)
  .subscribe(paged => this.doctors = paged.items);
```

If not unwrapped:

```ts
this.api.get('/api/doctors', filters)
  .subscribe((res: any) => this.doctors = res?.items ?? []);
```

## AuthService

`AuthService` should expose:
- `getToken()` — returns stored JWT
- `saveTokens(accessToken, refreshToken)` — persist
- `logout()` — clear tokens and navigate to login
- `getUserRole()` — return current user's role string

JwtInterceptor reads `auth.getToken()` to attach `Authorization` header.

## SignalR

Create `SignalRService` to manage connection and events. Expose `onNotification()`, `onMessage()` as Observables.

## Pipes & Directives

- `TimeAgoPipe` added for chat timestamps.
- `HasRoleDirective` enables role-based visibility in templates: `<button *hasRole="'Doctor'">New Slot</button>`

## File map

- Shared components: `shared/components/*`
- Pipes: `shared/pipes/time-ago.pipe.ts`
- Directive: `shared/directives/has-role.directive.ts`


---

If you want, I can also update `ApiService` to automatically unwrap `ApiResponse<T>` across the app.
