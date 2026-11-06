# Especificación — Portal Mi Salud

Fecha: 20/10/2026

Documento de requisitos funcionales y no funcionales del portal de pacientes (Blazor Server), en formato EARS. Describe el comportamiento **implementado**.

**Sistema:** Portal Mi Salud  
**Actor principal:** paciente titular autenticado  
**Actor derivado:** titular operando el perfil de un hijo o hija menor de 13 años (perfil activo, sin un segundo inicio de sesión)

---

## 1. Requisitos funcionales

### 1.1 Cuenta e identidad

- **RF-1.** Cuando el paciente envía credenciales válidas, el Portal Mi Salud deberá autenticarlo contra Andes y emitir una cookie de sesión `MiSalud` con expiración deslizante de una hora.
- **RF-2.** Cuando el paciente inicia sesión, el Portal Mi Salud deberá almacenar el JWT de Andes y los datos de identidad del paciente (identificador MPI, documento, nombre, sesión) como claims de la cookie de sesión.
- **RF-3.** Cuando el paciente solicita cerrar sesión, el Portal Mi Salud deberá invalidar la cookie de sesión y redirigirlo a la página de ingreso.
- **RF-4.** Cuando un visitante no autenticado intenta acceder a una página protegida, el Portal Mi Salud deberá redirigirlo a la página de ingreso con la URL de retorno.
- **RF-5.** Cuando un visitante completa el registro con datos personales, el Portal Mi Salud deberá validar la identidad contra RENAPER vía X-Road y, si corresponde, registrar la cuenta en Andes.
- **RF-6.** Cuando el visitante ingresa un código de activación válido, el Portal Mi Salud deberá permitir crear o restablecer la contraseña según el flujo iniciado.
- **RF-7.** Cuando el visitante solicita recuperar la contraseña con un correo registrado, el Portal Mi Salud deberá solicitar a Andes el envío del flujo de recuperación con origen “Portal Mi Salud”.
- **RF-8.** Cuando el visitante restablece la contraseña con un código válido, el Portal Mi Salud deberá actualizar la contraseña en Andes.

### 1.2 Inicio y navegación

- **RF-9.** Cuando un visitante autenticado abre la raíz del sitio, el Portal Mi Salud deberá redirigirlo a la página de inicio; cuando no está autenticado, deberá redirigirlo a la página de ingreso.
- **RF-10.** Cuando el paciente autenticado abre la página de inicio, el Portal Mi Salud deberá mostrar el aviso de privacidad sobre el acceso a la historia clínica.
- **RF-11.** Si la validación de conexión con el gateway X-Road (RENAPER) falla al abrir el inicio, entonces el Portal Mi Salud deberá informar al paciente con un aviso visible.
- **RF-12.** Mientras el paciente está autenticado, el Portal Mi Salud deberá ofrecer navegación a turnos, laboratorios, vacunas, historia de salud, recetas, farmacias de turno, centros de salud, grupo familiar, mis datos, mis programas y cierre de sesión.

### 1.3 Mis datos

- **RF-13.** Cuando el paciente titular abre Mis datos, el Portal Mi Salud deberá mostrar y permitir editar los datos de contacto y domicilio del titular (alias, género, domicilio, correo, celular validado) y persistirlos en Andes.
- **RF-14.** Mientras hay un perfil familiar activo, el Portal Mi Salud deberá mostrar y permitir editar en Mis datos los datos de contacto y domicilio del familiar (no del titular) y persistirlos en Andes.

### 1.4 Grupo familiar y perfil activo

- **RF-15.** Cuando el paciente titular abre Grupo familiar, el Portal Mi Salud deberá listar las relaciones familiares asociadas a su MPI en Andes.
- **RF-16.** Cuando el titular selecciona un familiar elegible (relación hijo o hija y edad menor a 13 años), el Portal Mi Salud deberá activar ese perfil, persistirlo en almacenamiento local protegido y permitir operar turnos, laboratorios, Mis datos, historia de salud, recetas y mis programas en nombre del familiar.
- **RF-17.** Mientras hay un perfil familiar activo, el Portal Mi Salud deberá mostrar en el menú de cuenta el nombre del familiar (con estilo visual distinto al del titular) y, debajo del nombre, un botón para volver al perfil del titular. El ítem Actuar como un familiar deberá abrir un diálogo con texto explicativo sobre dónde se ven los datos del familiar y la lista del titular y los familiares elegibles para seleccionar el perfil activo; Grupo familiar podrá activar un perfil sin redirigir a turnos.
- **RF-18.** Si Andes rechaza una operación de agenda o telesalud por falta de autorización sobre el familiar (401/403), entonces el Portal Mi Salud deberá informar al titular con un mensaje específico de esa condición.

### 1.5 Turnos

- **RF-19.** Cuando el paciente (perfil activo) abre Mis turnos, el Portal Mi Salud deberá listar los turnos futuros, incluyendo telemedicina dentro de la ventana de visualización configurada.
- **RF-20.** Cuando el paciente abre el historial de turnos, el Portal Mi Salud deberá listar los turnos pasados, manteniendo visibles las videoconferencias hasta el fin de la ventana configurada.
- **RF-21.** Cuando el paciente solicita un turno presencial, el Portal Mi Salud deberá ofrecer agendas disponibles según georreferencia del domicilio, excluir horarios en colisión con turnos activos y registrar el turno en Andes tras confirmación.
- **RF-22.** Cuando el paciente solicita teleconsulta, el Portal Mi Salud deberá exigir el consentimiento de telesalud en la visita, limitar a un telesalud activo y enviar la solicitud con motivo y teléfono de contacto.
- **RF-23.** Cuando el paciente confirma la cancelación de un turno, el Portal Mi Salud deberá cancelar el turno en Andes para el paciente del perfil activo.
- **RF-24.** Cuando una acción de turno finaliza (confirmación, no confirmación o cancelación), el Portal Mi Salud deberá mostrar la página de mensaje correspondiente.

### 1.6 Laboratorios

- **RF-25.** Cuando el paciente (perfil activo) abre Mis laboratorios, el Portal Mi Salud deberá fusionar y ordenar por fecha los resultados provenientes de Andes (RUP/CDA) y de RANIA, dentro del rango de fechas configurado.
- **RF-26.** Cuando el paciente solicita la descarga de un informe de laboratorio, el Portal Mi Salud deberá entregar el PDF según el tipo de origen (RUP, CDA o RANIA).
- **RF-27.** Mientras el listado de laboratorios tiene más páginas, el Portal Mi Salud deberá cargar resultados adicionales mediante desplazamiento continuo según el tamaño de página configurado.

### 1.7 Historia de salud, recetas y vacunas

- **RF-28.** Cuando el paciente (perfil activo) abre Historia de salud, el Portal Mi Salud deberá mostrar las categorías SNOMED disponibles y, al seleccionar una, el listado de prestaciones asociadas al paciente del perfil activo.
- **RF-29.** Cuando el paciente solicita PDF, imagen o enlace de una prestación, el Portal Mi Salud deberá obtener el recurso desde Andes (usando el paciente del perfil activo cuando corresponda) y registrar la telemetría correspondiente de descarga, visualización o copia.
- **RF-30.** Cuando el paciente (perfil activo) abre Mis recetas, el Portal Mi Salud deberá listar las recetas fusionadas de Andes y Recetar del paciente activo, ordenadas por fecha.
- **RF-31.** Cuando el paciente titular abre Vacunaciones, el Portal Mi Salud deberá listar las campañas y aplicaciones de vacunas obtenidas de Andes según el JWT del titular. Si el perfil activo es un familiar, no deberá listar vacunas ni consultar Andes (el endpoint no filtra por id de familiar).

### 1.8 Farmacias y centros de salud (titular)

- **RF-32.** Cuando el paciente abre Farmacias de turno, el Portal Mi Salud deberá listar las localidades disponibles y las farmacias de turno del día, con preselección según el domicilio del titular cuando corresponda.
- **RF-33.** Cuando el paciente abre Centros de salud, el Portal Mi Salud deberá mostrar en mapa los centros de la provincia o del área Araucanía según el origen seleccionado, usando la georreferencia del domicilio del titular.

### 1.9 Consentimientos y programas

- **RF-34.** Cuando el paciente (perfil activo) abre Mis programas, el Portal Mi Salud deberá listar los consentimientos de programas del paciente activo y permitir revisar la versión vigente y aceptar o rechazar.
- **RF-35.** Cuando un paciente de 65 años o más inicia sesión y aún no respondió el consentimiento Cuidar65, el Portal Mi Salud deberá evaluar elegibilidad y, si corresponde, presentar el aviso de consentimiento del programa.
- **RF-36.** Cuando el paciente acepta el consentimiento de telesalud en la página de teleconsulta, el Portal Mi Salud deberá permitir continuar con la solicitud de teleconsulta en esa visita.

### 1.10 Observabilidad

- **RF-37.** Donde `AdminLogs:ApiKey` está configurada, el Portal Mi Salud deberá enviar logs de aplicación (nivel Information o superior) y eventos de telemetría (`view_page`, `click`, `download_pdf`, `view_image`, `copy_link`) a la API de ingesta del panel administrativo, con correlación por `sessionId` y `patientId` cuando estén disponibles.
- **RF-38.** Si `AdminLogs:ApiKey` está vacía, entonces el Portal Mi Salud deberá operar sin enviar logs ni telemetría al panel administrativo.

---

## 2. Requisitos no funcionales

- **RNF-1.** El Portal Mi Salud deberá exponerse mediante HTTPS fuera del entorno de desarrollo y aplicar HSTS.
- **RNF-2.** El Portal Mi Salud deberá emitir la cookie de sesión con `Secure=Always` y `SameSite=Lax`.
- **RNF-3.** El Portal Mi Salud deberá conservar el JWT de Andes únicamente en el servidor (claims de la cookie) e inyectarlo en las llamadas HTTP autenticadas a Andes.
- **RNF-4.** El Portal Mi Salud deberá evitar el almacenamiento de URLs, tokens y secretos de backends en el código fuente versionado; la configuración operativa deberá cargarse por user-secrets, variables de entorno o archivos de entorno no versionados.
- **RNF-5.** Cuando el Portal Mi Salud presente fechas y horas de negocio (turnos, recetas, vacunas, farmacias, consentimientos, historial), deberá normalizarlas a la zona horaria de Argentina.
- **RNF-6.** El Portal Mi Salud deberá exigir la carga de `saludConfig.json` al arrancar y validar la sección `SaludConfiguracion`.
- **RNF-7.** Cuando el Portal Mi Salud renderice contenido HTML de consentimientos, deberá sanitizarlo antes de mostrarlo.
- **RNF-8.** Donde el envío al Admin está activo, el Portal Mi Salud deberá encolar eventos en una cola acotada con política de descarte de los más antiguos si se satura, y reintentar el envío con retroceso ante fallos temporales.
- **RNF-9.** El Portal Mi Salud deberá ejecutarse sobre .NET 9 como aplicación Blazor Server.
- **RNF-10.** Cuando ocurre un error no controlado en producción, el Portal Mi Salud deberá dirigir al paciente a la página de error del sitio.
- **RNF-11.** El Portal Mi Salud deberá exponer atributos de accesibilidad básicos en formularios y avisos críticos (etiquetas ARIA, regiones `aria-live` donde ya están implementadas).

---

## 3. Fuera de alcance (no implementado)

Los siguientes ítems **no** forman parte de los requisitos vigentes:

- Ruta `/turnos` como funcionalidad de producto (página marcador “En desarrollo”).
- Consumo de la API LASCHyBS en flujos del portal (el adaptador puede estar registrado sin uso).
- Actualización de vacunaciones vía API (método pendiente de implementación).
- Persistencia del consentimiento de telesalud entre visitas (solo estado de la visita actual).
- Uso del perfil familiar en vacunas (Andes solo expone vacunas del JWT del titular), farmacias de turno o centros de salud.

---

## 4. Integraciones externas (referencia)

- **Andes:** autenticación, paciente, turnos, laboratorios RUP/CDA, historia, recetas Andes, vacunas, farmacias, centros, consentimientos.
- **X-Road (RENAPER):** validación de identidad en registro y sonda de conectividad.
- **X-Road (RANIA):** resultados e informes de laboratorio Clínica Raña.
- **Recetar:** prescripciones por MPI.
- **SaludPortal.Admin:** ingesta opcional de logs y telemetría.
