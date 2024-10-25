ARQUITECTURA DE N CAPAS

1. Capa Entity: (Dominio / Entidades)
Responsabilidad: Esta capa contiene las entidades o modelos de dominio que representan los objetos de negocio.
Estas clases suelen ser simples, con propiedades que reflejan los datos que se almacenan y manipulan.

2. Capa DAL (Data Access Layer):
Responsabilidad: La Capa de Acceso a Datos maneja la interacción con la base de datos. Aquí es donde se 
implementan los repositorios y se define cómo se realizan las operaciones CRUD sobre las entidades de la 
capa Entity.

3. Capa BLL (Business Logic Layer):
Responsabilidad: La capa de lógica de negocio maneja las reglas de negocio y la lógica de aplicación 
que no es simplemente CRUD. Aquí es donde se implementan los servicios que manipulan las entidades,
aplicando reglas de negocio complejas.

4. Capa IOC (Inversion of Control / Dependency Injection):
Responsabilidad: Esta capa se encarga de la configuración de la inyección de dependencias y la resolución
de dependencias en la aplicación. Es donde se definen los contenedores de IoC (como Autofac, Simple Injector, 
o el propio contenedor de .NET Core) y se configura la forma en que se deben resolver las dependencias.

Ejemplo: Un contenedor IoC que registra los servicios de la capa BLL y DAL, configurando cómo se deben 
instanciar y gestionar las dependencias a lo largo de la aplicación.

5. Capa Aplicacion (Presentation Layer):
Responsabilidad: Esta capa contiene la interfaz de usuario y es la que interactúa directamente con el
usuario final. Aquí es donde se encuentran las aplicaciones web, móviles o de escritorio. La capa Aplicacion 
depende de los servicios expuestos por la capa BLL para realizar operaciones de negocio.