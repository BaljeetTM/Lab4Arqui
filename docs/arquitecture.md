# Laboratorio 4 - Arquitectura de Shortly -  Microservicios

## Justificación de la descomposición

El monolito tiene dos servicios (`UserService` y `LinkService`) pero con cargas de trabajo muy distintas. Por eso se separó en 4 servicios:

-Identity Service: sale de `UserService`/`UserRepository`. Maneja registro y login, es un dominio aparte (seguridad).
-URL Service: sale de la parte de `LinkService` que crea y lista enlaces. Es de baja frecuencia.
-Redirect Service: sale de `UrlRedirectEndpoint`. Es el que más tráfico recibe (cada clic pasa por acá), por eso necesita ser independiente y no depender de la base de datos relacional.
-Stats Service: sale de `IncrementClicks`. Hoy se ejecuta junto con el redirect; separarlo evita que contar clics le quite velocidad al redirect.

Se agregó un API Gateway como entrada única para no repetir la lógica de ruteo/autenticación en cada servicio.

## Patrones de comunicación

-Síncrona (HTTP/REST): el Gateway habla con los 4 servicios por HTTP/REST, porque son peticiones normales de request-respuesta.
-Asíncrona (mensajería): Redirect Service no llama directo a Stats Service, publica un evento a un message broker (RabbitMQ/Kafka). Así el redirect responde rápido sin esperar a que se guarde el clic.
-URL Service actualiza el cache de Redirect Service cuando se crea/edita un enlace, para que Redirect no dependa de la base de datos de URL Service.

## Propiedad de los datos

Cada servicio tiene su propia base de datos (database per service):

-Identity Service → BD usuarios (PostgreSQL)
-URL Service → BD enlaces (PostgreSQL)
-Redirect Service → Cache de enlaces (Redis)
-Stats Service → BD clics (PostgreSQL)

Como Redirect trabaja con una copia en cache, hay consistencia eventual entre crear un enlace y que esté disponible para redirigir. Lo mismo pasa con las estadísticas, que se procesan de forma asíncrona.

## Consideraciones de escalabilidad

-Redirect Service es el que más necesita escalar solo, porque recibe el mayor tráfico y no tiene estado propio.
-Identity Service tiene un problema si se escala: el rate limiting de login fallido hoy se guarda en memoria (`ConcurrentDictionary`), y eso no se comparte entre réplicas. Habría que moverlo a Redis.
-Stats Service puede escalar según cuántos eventos de clic lleguen, sin afectar al resto.
-URL Service es el que menos necesita escalar, porque crear enlaces es poco frecuente comparado con las redirecciones.

## Modos de falla

-Si cae Redirect Service, afecta a todos los visitantes. Se soluciona con varias réplicas detrás del Gateway.
-Si cae el cache Redis, Redirect no podría resolver ninguna URL corta. Se puede agregar un fallback a la base de datos de URL Service en ese caso.
-Si cae el broker o Stats Service, no afecta al redirect, solo se pierden o atrasan las estadísticas. Es aceptable como degradación.
-Si cae Identity Service, no se puede hacer login/registro, pero los usuarios ya logueados y los visitantes siguen funcionando normal.
-El API Gateway es punto único de falla, por eso conviene tener más de una instancia.

## Stack tecnológico propuesto

-Framework: ASP.NET Core (Minimal APIs), para seguir con lo mismo del monolito.
-Base de datos: PostgreSQL en vez de SQLite, para producción.
-Cache: Redis, para el lookup de Redirect y también para mover ahí el rate limiting de Identity.
-Mensajería: RabbitMQ o Kafka para los eventos de clic.
-API Gateway: YARP, por ser nativo de .NET.
-Autenticación: JWT emitido por Identity Service, en vez de cookies + `MemoryCacheTicketStore`, para que el Gateway no dependa de una llamada síncrona a Identity en cada request.