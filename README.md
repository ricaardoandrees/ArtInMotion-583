# ArtInMotion

Proyecto web con:
- `back-end`: API en ASP.NET Core (`net9.0`)
- `front-end`: vistas HTML/CSS/JS servidas por el backend

## Requisitos

- [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Windows, macOS o Linux
- Navegador web moderno (Chrome, Edge, Firefox)

## Estructura del proyecto

- `proyecto.sln`
- `back-end/` (API + configuración de archivos estáticos)
- `front-end/` (interfaz del usuario)

## Ejecutar el proyecto (desarrollo)

1. Abre una terminal en la raíz del repositorio.
2. Restaura dependencias:

```bash
dotnet restore "proyecto.sln"
```

3. Ejecuta el backend:

```bash
dotnet run --project "back-end/back-end.csproj"
```

4. Abre en el navegador:
- App (login): `http://localhost:5289/front-end/viewUsuario/login.html`
- Catálogo: `http://localhost:5289/front-end/viewCatalogo/index.html`

> Nota: El frontend usa URLs fijas a `http://localhost:5289`, por lo que debes ejecutar en ese puerto.

## Ejecutar desde Visual Studio

1. Abre `proyecto.sln`.
2. Selecciona el proyecto `back-end` como Startup Project.
3. Ejecuta con el perfil `http`.
4. Navega a `http://localhost:5289/front-end/viewUsuario/login.html`.

## Endpoints útiles para verificación rápida

- `http://localhost:5289/api/catalogo`
- `http://localhost:5289/api/usuarios/search`

## Problemas comunes

- **No abre la app o falla conexión:** verifica que el backend esté corriendo en `http://localhost:5289`.
- **Error de SDK:** ejecuta `dotnet --info` y confirma que tienes SDK `9.x`.
- **Puerto ocupado:** libera el puerto `5289` o ajusta `launchSettings.json` y también las URLs del frontend.

