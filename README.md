# Desarrollo Web – Aplicación Web Baloncesto · Documentación externa
## Integrantes: Jenny Sofia Morales López 7690 08 6790 y Cristian Alejandro Melgar Ordoñez 7690 21 8342
---

## Tabla de contenido
1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Alcance y Objetivos](#alcance-y-objetivos)
3. [Arquitectura](#arquitectura)
4. [Requerimientos](#requerimientos)
5. [Dominio del Negocio](#dominio-del-negocio)
6. [Diseño UX/UI](#diseño-uxui)
7. [Arquitectura Técnica](#arquitectura-técnica)
8. [API Backend (.NET 8 + EF Core)](#api-backend-net-8--ef-core)
9. [Frontend (Angular 20)](#frontend-angular-20)
10. [Seguridad y Cumplimiento](#seguridad-y-cumplimiento)
11. [Pruebas y Aseguramiento de Calidad](#pruebas-y-aseguramiento-de-calidad)
12. [Glosario](#glosario)
13. [Apéndices](#apéndices)

---

## Resumen Ejecutivo
Aplicación web para gestión de torneos y partidos de baloncesto: administración de equipos y jugadores, programación de encuentros torneos y amistosos, control en tiempo real (marcador, faltas, tiempos, cuartos), historial y estadísticas. Arquitectura **Angular 20** + **API REST .NET 8** + **SQL Server**. Despliegue con **Docker** e instalación en **VPS**. Seguridad basada en **JWT**.

**Dominio registrado: basketmarcador.online**

**IP Pública 91.99.197.226**

**Clonar Repositorios**
- Repositorio Frontend: https://github.com/sofiaMorales3805/MarcadorAngularFront.git
- Repositorio Backend: https://github.com/sofiaMorales3805/MarcadorFaseIIBack.git 

**Correr Frontend**
- npm install
- ng serve

**Correr Backend**
- dotnet ef database update
- dotnet build
- dotnet run


---

## Alcance y Objetivos
**Objetivo general**: entregar una plataforma usable y segura para administrar torneos y controlar partidos en vivo.

**Objetivos específicos**:
- CRUD de **equipos**, **jugadores**, **torneos**, **partidos** y **rondas**.
- **Control de tablero**: reloj de juego, periodos, faltas, tiempos fuera, marcador.
- **Historial** con filtros por torneo, ronda, equipos y fecha.
- **Autenticación** y **autorización por rol** (admin, operador, visor).
- **Despliegue reproducible** con Docker e instalación en VPS.

---

## Arquitectura
**Tipo de arquitectura**
Estilo: Cliente–servidor.

Capas:
Presentación ,
Aplicación/Lógica (API ASP.NET Core, monolito modular),
Datos (SQL Server).
Despliegue: Contenedores Docker; 

---

## Requerimientos
### Funcionales
- **Autenticación/Autorización** (login, logout, expiración). Roles: `admin`, `operador`, `visualizador`.
- **Gestión de Equipos**: crear/editar/eliminar, búsqueda y filtros; logo opcional.
- **Gestión de Jugadores** por equipo.
- **Gestión de Torneos**: nombre, temporada, estado; **Rondas** (ej.: fase, semifinal, final).
- **Gestión de Partidos**: amistosos y de torneo; asignación de equipos, ronda, fecha/hora y estado.
- **Control del Partido**: marcador + reloj + periodos + faltas + tiempos fuera.
- **Historial**: listado, filtros.

### No funcionales
- **Disponibilidad** ≥ 99% (objetivo en horario hábil).
- **Seguridad**: JWT, TOKEN REFRESH.

---

## Dominio del Negocio
**Entidades principales**: Equipo, Jugador, Torneo, Ronda, Partido, Incidencia (puntaje/falta/tiempo), Usuario, Rol.

**Casos de uso**:
- Admin crea torneo y configura rondas.
- Operador agenda un partido, lo opera en vivo y finaliza.
- Usuario consulta historial con filtros.

---

## Diseño UX/UI
**Principios**: consistencia, jerarquía visual clara, accesibilidad, diseño responsive.

**Componentes clave**: navbar con control de visibilidad (oculto en `/login`), tablas con filtro y paginación, formularios con validación visual, panel de control del partido con estados claros (reloj, periodos, faltas).

---

## Arquitectura Técnica
### Tecnologías
- **Frontend**: Angular 20, TypeScript, Angular Material, SCSS.
- **Backend**: .NET 8, ASP.NET Core, EF Core..
- **BD**: SQL Server 2022.
- **Infra**: Docker, Nginx .
---

## API Backend (.NET 8 + EF Core)
**Base URL**: `/api`

**Códigos y errores estándar**: 200, 201, 204, 400 (validación), 401 (no autenticado), 403 (sin permisos), 404 (no encontrado), 409 (conflicto), 422 (semántico), 500 (interno).

### Migraciones EF Core
```bash
# agregar migración
 dotnet ef migrations add Init --project MarcadorFaseIIApi
# aplicar cambios
 dotnet ef database update
# revertir última
 dotnet ef migrations remove
```

---

## Frontend (Angular 20)
### Estructura
```
src/
  app/
    core/ (auth.service, interceptors, guards)
    features/
      equipos/
      jugadores/
      torneos/
      partidos/
      control/
      historial/
```

### Configuración
- Interceptor JWT añade `Authorization: Bearer <token>`.
- **Navbar** oculto en `/login` (control por ruta/guard).

### Comandos
```bash
# desarrollo
npm install
ng serve -o
# producción
ng build --configuration production
```
---

## Seguridad y Cumplimiento
- **JWT**: expiración + refresh opcional; revocación en server si aplica.
- **Hash contraseñas**: PBKDF2/BCrypt/Argon2.
- **CORS** restringido a orígenes conocidos.
- **TLS**: HSTS, redirección 80→443, TLS1.2+.
- **Roles** en claims; autorización por **policy**.

---

## Pruebas y Aseguramiento de Calidad
- **Unitarias**: servicios/validadores (Front), servicios/aplicación (Back).
- **Integración**: endpoints críticos (login, partidos, historial).
- **E2E**: flujos clave (crear torneo→partido→operar→finalizar→ver historial).
- **Caja negra** (resumen):
  - **Partición de equivalencia** y **Valores límite** en filtros (fechas, puntajes).
  - **Tablas de decisión** para reglas de finalización de partido.
  - **Transición de estados**: `programado → en_juego → finalizado`.

---

## Glosario
- **Operador**: usuario que controla el partido en vivo.
- **Ronda**: fase del torneo (ej.: Cuartos, Semifinal, Final).
- **Incidencia**: evento registrado en un partido (punto, falta, tiempo fuera).

---

### Apéndices
**A. Reglas de negocio clave**
- Un partido **amistoso** no requiere torneo/ronda.
- Un partido **de torneo** requiere `TorneoId` y opcional `RondaId`.
- Estados válidos: `programado`, `en_juego`, `suspendido`, `finalizado`.
