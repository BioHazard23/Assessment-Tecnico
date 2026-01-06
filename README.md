# Plataforma de Cursos - Assessment Técnico

## 📋 Visión General
Esta es una plataforma full-stack para la gestión y publicación de cursos online. El sistema ha sido diseñado siguiendo los principios de **Clean Architecture** para asegurar la independencia de frameworks, testabilidad y escalabilidad.

## 🏗 Arquitectura del Sistema

El backend (.NET 8) está estructurado en 4 capas concéntricas:

### 1. Domain (Núcleo)
*   Contiene las Entidades (`Course`, `Lesson`) y Enums (`CourseStatus`).
*   Define las reglas de negocio empresariales y excepciones de dominio.
*   **Sin dependencias externas**.

### 2. Application (Casos de Uso)
*   Orquesta la lógica de la aplicación interactuando con el Dominio.
*   Implementa casos de uso específicos como `PublishCourseUseCase`, `ReorderLessonsUseCase`.
*   Define interfaces (`ICourseRepository`, `IUnitOfWork`) que la infraestructura debe implementar.
*   Usa DTOs para aislar el modelo de dominio del mundo exterior.

### 3. Infrastructure (Adaptadores)
*   Implementación de persistencia con **EF Core** y PostgreSQL.
*   Configuración de **Identity** para autenticación y autorización.
*   Implementación de repositorios y servicios externos (JWT).

### 4. API (Framework)
*   Punto de entrada de la aplicación (Controllers).
*   Configuración de Inyección de Dependencias y Middleware.
*   Exposición de endpoints RESTful.

El **Frontend** es una SPA desarrollada con **React + Vite**, enfocada en una experiencia de usuario fluida y moderna.

---

## 🚀 Guía de Inicio Rápido

### Opción A: Ejecución Local (Recomendada para desarrollo)

**1. Base de Datos**
Asegúrate de tener un servidor PostgreSQL corriendo y actualiza la cadena de conexión en `src/Api/appsettings.json` si es necesario.

**2. Backend (API)**
Desde la carpeta raíz del proyecto, abre una terminal:
```bash
# Navegar a la carpeta de la API
cd src/Api

# Ejecutar migraciones (Crear BD y Tablas)
dotnet ef database update

# Iniciar el servidor
dotnet run
```
La API estará disponible en: [http://localhost:5050](http://localhost:5050)

**3. Frontend (React)**
Desde otra terminal:
```bash
# Navegar a la carpeta del frontend
cd frontend

# Instalar dependencias
npm install

# Iniciar modo desarrollo
npm run dev
```
La aplicación web abrirá en: [http://localhost:5173](http://localhost:5173)

### Opción B: Docker Compose (Despliegue completo)

Si prefieres no instalar dependencias locales (.NET SDK, Node.js, Postgres), puedes usar Docker:

```bash
# Construir y levantar todo el entorno
docker-compose up --build
```

Esto levantará 3 servicios:
*   **API**: [http://localhost:5000](http://localhost:5000)
*   **Base de Datos**: Puerto 5432 (interno)
*   **Frontend**: [http://localhost:3000](http://localhost:3000)

> **Nota:** La configuración de puertos en Docker es diferente a la ejecución local para evitar conflictos.

---

## 📡 Documentación de Endpoints

### 🔐 Autenticación (`Auth`)
| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `POST` | `/api/auth/register` | Registro de nuevos usuarios |
| `POST` | `/api/auth/login` | Inicia sesión y devuelve JWT |

### 📚 Cursos (`Courses`)
| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/courses/search` | Buscar cursos (filtros: query, status, pagina) |
| `POST` | `/api/courses` | Crear un nuevo curso (Draft por defecto) |
| `PUT` | `/api/courses/{id}` | Actualizar título del curso |
| `DELETE` | `/api/courses/{id}` | Soft Delete (Papelera) |
| `DELETE` | `/api/courses/{id}/hard` | Hard Delete (**Solo Admin**) |
| `PATCH` | `/api/courses/{id}/publish` | Publicar curso (Requiere lecciones) |
| `PATCH` | `/api/courses/{id}/unpublish` | Despublicar (volver a Draft) |

### 📝 Lecciones (`Lessons`)
| Método | Endpoint | Descripción |
| :--- | :--- | :--- |
| `GET` | `/api/courses/{id}/lessons` | Listar lecciones de un curso |
| `POST` | `/api/courses/{id}/lessons` | Agregar lección |
| `PATCH` | `/api/courses/{id}/lessons/{id}/move-up` | Reordenar hacia arriba |
| `PATCH` | `/api/courses/{id}/lessons/{id}/move-down` | Reordenar hacia abajo |

---

## 🔑 Credenciales de Prueba

El sistema incluye usuarios pre-creados para facilitar las pruebas:

| Rol | Email | Password |
| :--- | :--- | :--- |
| **Usuario Estándar** | `test@test.com` | `Test123!` |
| **Administrador** | `admin@test.com` | `Admin123!` |

> **Nota**: El usuario Administrador tiene permisos exclusivos para realizar "Hard Delete" (borrado físico).

---

## 🛠 Features y Decisiones Técnicas

*   **Soft Delete**: Las entidades `Course` y `Lesson` implementan borrado lógico (`IsDeleted`) para preservación de datos. EF Core filtra automáticamente estos registros globalmente.
*   **Validaciones de Dominio**:
    *   Un curso no puede publicarse si no tiene lecciones activas.
    *   El orden de las lecciones se gestiona con lógica de intercambio seguro para evitar colisiones de índices únicos.
*   **Autenticación**: JWT (JSON Web Tokens) con expiración configurable.
*   **UI/UX**: Diseño moderno "Warm Palette", totalmente responsivo y localizado al español.

## 🧪 Testing

El proyecto incluye una suite de pruebas unitarias con **xUnit** y **Moq** para validar la lógica de negocio crítica en la capa de Aplicación.

Para ejecutar los tests:
```bash
dotnet test
```
