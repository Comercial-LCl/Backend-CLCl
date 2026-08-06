# FacturasIA.Platform — Backend

Backend del **Sistema Inteligente de Registro y Análisis de Facturas**, un sistema web que permite a pequeños negocios registrar automáticamente sus facturas de compra mediante código QR + foto, o carga de PDF, usando OCR e inteligencia artificial para minimizar el ingreso manual de datos y dar visibilidad del gasto por categoría.

Proyecto académico individual — UPC, curso de Desarrollo de Aplicaciones Open Source.

🔗 **API en producción:** https://backend-clcl.onrender.com
📄 **Swagger:** https://backend-clcl.onrender.com/swagger/index.html
🖥️ **Frontend:** https://github.com/Comercial-LCl/Front-CLCl · https://front-clcl.vercel.app

> ⚠️ El backend corre en el free tier de Render, que duerme tras 15 min sin tráfico — la primera petición después de eso puede tardar 30-60s (cold start).

---

## Stack tecnológico

| Capa | Tecnología |
|---|---|
| Framework | ASP.NET Core (.NET 9), C# |
| Base de datos | PostgreSQL (Neon, serverless) |
| ORM | Entity Framework Core + Npgsql |
| Autenticación | JWT (hasheo de contraseñas con BCrypt) |
| Documentación | Swagger / OpenAPI (Swashbuckle) |
| OCR + IA | Google Gemini API (multimodal) |
| Consulta de RUC | Decolecta (proxy sobre el padrón de SUNAT) |
| Almacenamiento de archivos | Oracle Object Storage (API S3-compatible, vía AWSSDK.S3) |
| Extracción de texto de PDF | PdfPig |
| Hosting | Render.com |

## Arquitectura

DDD / Clean Architecture con CQRS y manejo de errores vía `Result<T>`, organizado en **Bounded Contexts**, cada uno con sus 4 capas (`Domain`, `Application`, `Infrastructure`, `Interfaces`):

- **Shared** — infraestructura común: `Result<T>`, `IBaseRepository`/`IUnitOfWork`, `AppDbContext`, manejo de `ProblemDetails`.
- **Iam** — autenticación y gestión de usuarios (JWT, BCrypt).
- **Invoicing** — el dominio de negocio: proveedores, categorías, productos, facturas y su procesamiento con IA.

Todos los IDs son `Guid`. Los repositorios siguen el patrón repository + unit of work; los casos de uso se organizan en comandos (escritura) y queries (lectura), cada uno con su propio servicio de aplicación.

## Features

- **Autenticación** — registro e inicio de sesión con JWT.
- **Registro de factura física** — datos de cabecera desde el QR (RUC, serie, número, fecha, monto) + foto opcional de los productos, procesada con Gemini para extraer ítems y clasificar la compra.
- **Registro de factura electrónica** — carga de PDF, extracción de texto y procesamiento con Gemini para obtener cabecera, ítems, categoría y resumen.
- **Consulta de RUC** — al leer el QR, se puede consultar el nombre de la empresa antes de completar el registro.
- **Corrección manual** — si la IA se equivoca en algún dato, se puede corregir después de registrada la factura.
- **Confianza de la IA por campo** — cada factura procesada con IA guarda el nivel de confianza (alta/media/baja) por campo extraído, para que el usuario sepa qué revisar.
- **Catálogo de productos e historial de precios** — la IA normaliza el nombre de cada producto comprado a un proveedor, permitiendo consultar cómo varió su precio entre compras.
- **Resumen de gastos** — por categoría y por periodo.

## Endpoints principales

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/v1/authentication/sign-up` | Registro de usuario |
| POST | `/api/v1/authentication/sign-in` | Inicio de sesión (devuelve JWT) |
| POST | `/api/v1/facturas/fisica` | Registrar factura física (QR + foto opcional) |
| POST | `/api/v1/facturas/electronica` | Registrar factura electrónica (PDF) |
| GET | `/api/v1/facturas` | Listar facturas del usuario |
| GET | `/api/v1/facturas/{id}` | Detalle de una factura |
| GET | `/api/v1/facturas/filtrar` | Filtrar por proveedor, categoría y/o fechas |
| PATCH | `/api/v1/facturas/{id}/corregir` | Corregir datos leídos mal por la IA |
| GET | `/api/v1/resumen-gastos/por-categoria` | Total de gastos por categoría |
| GET | `/api/v1/resumen-gastos/por-periodo` | Total de gastos en un rango de fechas |
| GET | `/api/v1/proveedores` | Listar catálogo de proveedores |
| GET | `/api/v1/proveedores/consultar-ruc/{ruc}` | Consultar empresa por RUC (SUNAT/Decolecta) |
| GET | `/api/v1/proveedores/{proveedorId}/productos` | Productos comprados a un proveedor |
| GET | `/api/v1/productos/{productoId}/historial-precios` | Historial de precios de un producto |
| GET | `/api/v1/categorias` | Listar catálogo de categorías |

Documentación completa e interactiva en `/swagger/index.html`.

## Cómo correrlo localmente

**Requisitos:** .NET 9 SDK, una base de datos PostgreSQL (Neon o local).

1. Clona el repositorio y restaura los paquetes:
```bash
   dotnet restore
```
2. Configura los secretos con `dotnet user-secrets` (nunca en `appsettings.json`):
```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<tu connection string>"
   dotnet user-secrets set "ConnectionStrings:Migrations" "<connection string sin pooler, para EF Core>"
   dotnet user-secrets set "TokenSettings:Secret" "<secreto largo y aleatorio>"
   dotnet user-secrets set "Gemini:ApiKey" "<tu api key de Google AI Studio>"
   dotnet user-secrets set "Decolecta:Token" "<tu token de Decolecta>"
```
   (Las credenciales de Oracle Object Storage van en `appsettings.Development.json` o como secrets adicionales, según tu entorno.)
3. Aplica las migraciones:
```bash
   dotnet ef database update
```
4. Corre el proyecto:
```bash
   dotnet run
```

## Variables de entorno en producción

En Render, los mismos secretos se configuran como variables de entorno con formato `Seccion__Clave` (doble guion bajo en vez de `:`), por ejemplo `ConnectionStrings__DefaultConnection`, `Gemini__ApiKey`, `TokenSettings__Secret`.
