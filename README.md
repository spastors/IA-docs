# Apuntes de inteligencia artificial

Notas breves y organizadas para repasar los conceptos de inteligencia artificial (IA).

## Índice

- [1. Orígenes y fechas clave](#1-orígenes-y-fechas-clave)
- [2. Modelos de lenguaje de gran tamaño (LLM): historia y evolución](#2-modelos-de-lenguaje-de-gran-tamaño-llm-historia-y-evolución)
- [3. PLN: procesamiento del lenguaje natural](#3-pln-procesamiento-del-lenguaje-natural)
- [4. Modelo de n-gramas](#4-modelo-de-n-gramas)
- [5. Contexto](#5-contexto)
- [6. Tokens](#6-tokens)
- [7. Determinismo y aleatoriedad](#7-determinismo-y-aleatoriedad)
- [8. Redes neuronales](#8-redes-neuronales)

## 1. Orígenes y fechas clave

### 1943: McCulloch y Pitts

**Warren S. McCulloch y Walter Pitts** publican un modelo matemático de neuronas inspirado en el sistema nervioso. Su trabajo es un antecedente fundamental de las redes neuronales artificiales.

Fuente: [A Logical Calculus of the Ideas Immanent in Nervous Activity (1943)](https://home.csulb.edu/~cwallis/382/readings/482/mccolloch.logical.calculus.ideas.1943.pdf).

### 1950: Alan Turing y el test de Turing

En su artículo *Computing Machinery and Intelligence*, Alan Turing planteó el «juego de imitación»: evaluar si una máquina puede mantener una conversación escrita que un evaluador confunda con la de una persona.

**Qué evalúa:** el comportamiento conversacional de la máquina. Superar esta prueba no demuestra por sí solo consciencia o comprensión humana.

Fuente: [Artículo original de Turing (1950)](https://courses.cs.umbc.edu/471/papers/turing.html).

### 1955: el término «inteligencia artificial»

**John McCarthy** introdujo el término en la propuesta del encuentro de Dartmouth, junto con Marvin Minsky, Nathaniel Rochester y Claude Shannon.

Fuente: [Propuesta original de Dartmouth (1955)](https://www-formal.stanford.edu/jmc/history/dartmouth/dartmouth.html).

### 1956: nacimiento formal de la IA como campo de investigación

El encuentro de Dartmouth, celebrado en el verano de 1956, se considera el nacimiento formal de la IA como campo de investigación, aunque existían trabajos anteriores.

**Para recordar:** 1950 → Turing; 1955 → aparece el término; 1956 → encuentro de Dartmouth.

Fuente: [Dartmouth: los orígenes de la IA](https://ai.dartmouth.edu/founding-and-future-ai).

## 2. Modelos de lenguaje de gran tamaño (LLM): historia y evolución

### Qué es un LLM

**LLM** significa *Large Language Model*: **modelo de lenguaje de gran tamaño**. Es una red neuronal entrenada con grandes cantidades de texto para aprender patrones del lenguaje y realizar tareas como resumir, traducir, responder preguntas o generar contenido.

Muchos LLM actuales utilizan la arquitectura **Transformer**. Los modelos generativos de la familia GPT producen texto prediciendo el siguiente token a partir del contexto disponible.

Los **parámetros** son los valores numéricos internos que se ajustan durante el entrenamiento. El número de parámetros describe parte del tamaño del modelo; los tokens miden las unidades de texto que procesa.

Fuentes: [Stanford: introducción a la IA](https://uit.stanford.edu/service/techtraining/ai-demystified/introduction) y [GPT-3: modelo autorregresivo y parámetros](https://arxiv.org/abs/2005.14165).

### Cronología

| Fecha | Hito | Aportación |
| --- | --- | --- |
| **1943** | McCulloch y Pitts presentan un modelo lógico de neuronas. | Establecen un antecedente de las redes neuronales artificiales. [Artículo](https://home.csulb.edu/~cwallis/382/readings/482/mccolloch.logical.calculus.ideas.1943.pdf). |
| **1950** | Alan Turing publica *Computing Machinery and Intelligence*. | Plantea el juego de imitación, antecedente conceptual para evaluar el comportamiento inteligente de las máquinas. [Artículo](https://courses.cs.umbc.edu/471/papers/turing.html). |
| **Décadas de 1950–1990** | Desarrollo y uso de modelos estadísticos de lenguaje, incluidos los n-gramas. | Estiman probabilidades de secuencias; desde los años setenta adquieren especial relevancia en reconocimiento del habla. Sus antecedentes matemáticos se remontan a Markov y Shannon. [Historia de los n-gramas](https://web.stanford.edu/~jurafsky/slp3/3.pdf#page=21). |
| **2003** | Yoshua Bengio y colaboradores publican *A Neural Probabilistic Language Model*. | Aprenden representaciones numéricas de palabras y probabilidades mediante una red neuronal, facilitando la generalización a secuencias nuevas. [Artículo](https://jmlr.csail.mit.edu/papers/v3/bengio03a). |
| **Década de 2010** | Avance del aprendizaje profundo y los modelos neuronales de lenguaje. | Las redes recurrentes y otras técnicas mejoran el procesamiento de secuencias y preparan el desarrollo de modelos más generales. [Stanford](https://nlp.stanford.edu/~manning/dissertations/AbigailSeeThesis-augmented.pdf). |
| **2017** | Vaswani y colaboradores presentan *Attention Is All You Need* en NIPS 2017. | Introducen el Transformer, basado en mecanismos de atención, con resultados destacados en traducción automática. [Artículo](https://proceedings.neurips.cc/paper_files/paper/2017/file/3f5ee243547dee91fbd053c1c4a845aa-Paper.pdf). |
| **2018** | OpenAI presenta **GPT**, posteriormente conocido como **GPT-1**. | Combina el preentrenamiento generativo con la adaptación a tareas de lenguaje. [Publicación](https://openai.com/index/language-unsupervised/). |
| **2018** | Google presenta **BERT**. | Utiliza contexto a izquierda y derecha para aprender representaciones del lenguaje y mejorar tareas de comprensión. El artículo aparece en 2018 y se publica en NAACL en 2019. [Artículo](https://arxiv.org/abs/1810.04805). |
| **2019** | OpenAI presenta **GPT-2**. | Amplía la escala del modelo y mejora la generación de texto; su versión mayor tiene aproximadamente 1.500 millones de parámetros. [Publicación](https://openai.com/index/better-language-models/). |
| **2020** | Se publica **GPT-3**. | Su versión mayor tiene **175.000 millones de parámetros** y muestra la capacidad de realizar distintas tareas con instrucciones y ejemplos en el contexto, sin ajustar los parámetros para cada tarea. [Artículo](https://arxiv.org/abs/2005.14165). |
| **30 de noviembre de 2022** | Lanzamiento de **ChatGPT**. | Facilita el uso conversacional de modelos de lenguaje; su versión inicial está basada en la serie GPT-3.5. [Anuncio](https://openai.com/index/chatgpt/). |
| **2023** | Se presenta **GPT-4**. | Su informe técnico describe un modelo capaz de recibir texto e imágenes y generar texto. [Informe](https://arxiv.org/abs/2303.08774). |
| **2024–2025** | Expansión de sistemas multimodales y aplicaciones con herramientas. | Se extienden los flujos de trabajo con texto, imágenes, audio y vídeo, y los sistemas que realizan tareas de varios pasos. [Balance de 2025](https://developers.openai.com/blog/openai-for-developers-2025). |

### Artículos fundamentales

**Computing Machinery and Intelligence — Alan M. Turing (1950)**

- **Publicación:** revista *Mind*, volumen 59, número 236, páginas 433–460.
- **Idea principal:** estudiar la inteligencia de las máquinas mediante su comportamiento en una conversación escrita.
- **Relevancia:** antecedente del debate sobre inteligencia artificial y evaluación de sistemas conversacionales.
- **Lectura:** [texto completo](https://courses.cs.umbc.edu/471/papers/turing.html).

**Attention Is All You Need — Ashish Vaswani y colaboradores (2017)**

- **Publicación:** *Advances in Neural Information Processing Systems 30*, NIPS 2017.
- **Idea principal:** presentar el **Transformer**, una arquitectura que utiliza atención para relacionar los elementos de una secuencia.
- **Atención:** permite ponderar la información de otros tokens del contexto al calcular la representación de un token.
- **Relevancia:** facilita el entrenamiento en paralelo y sirve de base para familias posteriores como GPT y BERT. En la generación autorregresiva, el texto se sigue produciendo token a token.
- **Lectura:** [PDF del artículo](https://proceedings.neurips.cc/paper_files/paper/2017/file/3f5ee243547dee91fbd053c1c4a845aa-Paper.pdf).

### Multimodalidad y agentes

Un **modelo multimodal** puede trabajar con varias modalidades de información, como texto e imágenes. Las modalidades admitidas dependen del modelo y de la aplicación.

Un **agente basado en un LLM** combina el modelo con herramientas y un proceso de varios pasos: recibe un objetivo, decide acciones y utiliza los resultados para continuar. Por ejemplo, puede consultar un inventario, calcular necesidades de material y preparar una propuesta de compra.

Los sistemas que combinan modelos de lenguaje con acciones ya se investigaban en **2022**, como muestra el trabajo **ReAct**. Su desarrollo se amplía durante los años siguientes.

Fuentes: [GPT-4 y multimodalidad](https://arxiv.org/abs/2303.08774), [ReAct (2022)](https://arxiv.org/abs/2210.03629) y [evolución de herramientas y modalidades en 2025](https://developers.openai.com/blog/openai-for-developers-2025).

## 3. PLN: procesamiento del lenguaje natural

El **PLN**, conocido en inglés como **NLP** (*Natural Language Processing*), es el área de la IA dedicada a procesar, interpretar y generar lenguaje humano.

Sus tareas se agrupan en dos grandes áreas:

| Grupo de tareas | Objetivo | Ejemplo |
| --- | --- | --- |
| **Comprensión** | Interpretar la información o la intención de un mensaje. | Identificar qué pide una persona o si una opinión es positiva. |
| **Generación** | Producir lenguaje. | Redactar una respuesta, un resumen o una descripción. |

**Relación entre conceptos:** la IA es el campo general; el PLN se centra en el lenguaje; los LLM son una de las herramientas empleadas en ese campo.

Fuente: [Stanford HAI: qué es el PLN](https://hai.stanford.edu/ai-definitions/what-is-nlp).

## 4. Modelo de n-gramas

### Origen: cadenas de Markov

Las **cadenas de Markov**, desarrolladas por el matemático **Andréi Markov**, son una de las bases matemáticas de los modelos de n-gramas.

| Año | Aportación |
| --- | --- |
| **1906** | Markov inicia sus trabajos sobre secuencias de sucesos dependientes, origen de las cadenas que llevan su nombre. |
| **1913** | Analiza 20.000 letras de *Eugenio Oneguin*, de Aleksandr Pushkin, y estudia las probabilidades de vocales y consonantes a partir de las letras anteriores. |
| **1948** | Claude Shannon aplica aproximaciones de n-gramas al estudio estadístico del inglés. |

Las bases del modelado de secuencias con memoria limitada tienen **más de un siglo de historia** y son anteriores al nacimiento formal de la IA en 1956.

Fuentes: [David Link: los trabajos de Markov sobre el lenguaje](https://www.its.caltech.edu/~matilde/TracesOfTheMouth.pdf) y [Jurafsky y Martin: historia de los n-gramas](https://web.stanford.edu/~jurafsky/slp3/3.pdf#page=21).

### Suposición de Markov

En una cadena de Markov de primer orden, la probabilidad del siguiente estado depende del estado actual. Los modelos de n-gramas aplican esta idea al lenguaje: aproximan la probabilidad del siguiente token utilizando un historial corto.

| Modelo | Historial utilizado | Orden de dependencia |
| --- | --- | --- |
| **Bigrama** | 1 token anterior | Primer orden |
| **Trigrama** | 2 tokens anteriores | Segundo orden |
| **N-grama** | n − 1 tokens anteriores | Orden n − 1 |

### Definición

Un **n-grama** es una secuencia de *n* tokens consecutivos. En los siguientes ejemplos, cada palabra se trata como un token y se omite la puntuación.

Un **modelo de n-gramas** estima el siguiente token a partir de los **n − 1 tokens anteriores**, utilizando frecuencias del conjunto de entrenamiento.

| Modelo | Tokens por secuencia | Contexto para predecir | Ejemplo de secuencia |
| --- | --- | --- | --- |
| **Unigrama** | 1 | Ninguno | «brisa» |
| **Bigrama** | 2 | 1 token anterior | «brisa suave» |
| **Trigrama** | 3 | 2 tokens anteriores | «brisa suave mueve» |

### Secuencias consecutivas

Frase: **«La brisa suave mueve las hojas»**.

- **Bigramas:** «La brisa», «brisa suave», «suave mueve», «mueve las», «las hojas».
- **Trigramas:** «La brisa suave», «brisa suave mueve», «suave mueve las», «mueve las hojas».

Las secuencias se desplazan una posición cada vez y se solapan.

### Predicción con bigramas y trigramas

Conjunto de entrenamiento:

> La brisa suave mueve las hojas.
>
> La brisa suave mueve las cortinas.
>
> La brisa suave refresca el patio.
>
> Una voz suave transmite calma.

Las probabilidades se calculan dividiendo las apariciones de cada continuación entre las apariciones del contexto:

| Modelo | Contexto | Siguiente palabra | Frecuencia | Probabilidad |
| --- | --- | --- | --- | --- |
| Bigrama | «suave» | «mueve» | 2 de 4 | 50 % |
| Bigrama | «suave» | «refresca» | 1 de 4 | 25 % |
| Bigrama | «suave» | «transmite» | 1 de 4 | 25 % |
| Trigrama | «brisa suave» | «mueve» | 2 de 3 | ≈ 66,7 % |
| Trigrama | «brisa suave» | «refresca» | 1 de 3 | ≈ 33,3 % |
| Trigrama | «voz suave» | «transmite» | 1 de 1 | 100 % |

El trigrama distingue entre «brisa suave» y «voz suave». El bigrama solo considera «suave». Las probabilidades describen este conjunto de entrenamiento.

### Limitaciones

El contexto reducido dificulta recoger relaciones entre palabras alejadas. Al aumentar *n*, se necesitan más datos para observar las posibles combinaciones. Las secuencias no observadas requieren técnicas como el **suavizado**, que evita asignarles automáticamente una probabilidad de cero.

Fuente: [Jurafsky y Martin: modelos de lenguaje de n-gramas](https://web.stanford.edu/~jurafsky/slp3/3.pdf).

### «Amnesia» y memoria limitada

La **«amnesia»** es una forma intuitiva de describir la memoria limitada de un modelo de n-gramas: los tokens que quedan fuera de sus últimas **n − 1 posiciones** dejan de intervenir directamente en la predicción.

Ejemplo con un modelo de trigramas que trata cada palabra como un token:

> Clara vive en Toledo y mañana regresará a…
>
> Clara vive en Cuenca y mañana regresará a…

En ambos casos, el contexto utilizado para predecir es **«regresará a»**. El modelo asigna la misma distribución de probabilidades a la continuación, aunque la ciudad mencionada antes sea diferente.

La «amnesia» afecta al historial utilizado durante la predicción. Las estadísticas aprendidas durante el entrenamiento se conservan.

Consecuencias:

- Dificultad para mantener un tema o recordar datos mencionados varias frases antes.
- Posibles contradicciones en textos largos, aunque las secuencias cercanas resulten plausibles.
- Aumentar *n* amplía el contexto, pero exige más datos y combinaciones de tokens.

Fuente: [Jurafsky y Martin: suposición de Markov y contexto limitado](https://web.stanford.edu/~jurafsky/slp3/3.pdf#page=3).

## 5. Contexto

El **contexto** es la información que el modelo tiene disponible para interpretar una entrada o predecir la continuación de un texto.

Ejemplo: **«Se sentó en el banco»** puede referirse a un asiento; **«Ingresó dinero en el banco»**, a una entidad financiera. Las palabras que acompañan a «banco» ayudan a interpretar su significado.

- En un **bigrama**, el contexto de predicción es el token anterior.
- En un **trigrama**, son los dos tokens anteriores.
- En un **LLM generativo**, puede incluir instrucciones, mensajes anteriores, documentos aportados y texto ya generado, dentro de sus límites.

### Ventana de contexto

Es la cantidad máxima de tokens que el modelo puede considerar en una operación. Más contexto permite aportar más información, aunque no garantiza que el modelo utilice todos los detalles correctamente.

El contexto es información disponible durante el uso del modelo; el entrenamiento es el proceso previo en el que aprende patrones y ajusta sus parámetros.

Fuentes: [Stanford HAI: ventana de contexto](https://hai.stanford.edu/ai-definitions/what-is-a-context-window) y [Stanford: cómo funcionan los modelos de lenguaje](https://guides.library.stanford.edu/ai_research/understanding_ai).

## 6. Tokens

Un **token** es una unidad de texto que procesa el modelo. Puede corresponder a una palabra, parte de una palabra, un signo de puntuación u otra secuencia de caracteres.

La **tokenización** es el proceso de dividir el texto en estas unidades. Cada token se representa mediante un identificador numérico.

### Ejemplo de tokenización

Con un tokenizador sencillo que separa palabras y puntuación:

> «La luna brilla.» → [«La», «luna», «brilla», «.»] → **4 tokens**.

En un tokenizador de subpalabras, una palabra como «reorganización» podría dividirse en fragmentos. La división exacta depende del tokenizador; los tokens no tienen por qué coincidir con sílabas.

**Un token no equivale siempre a una palabra.** El número de tokens depende del texto, el idioma y el tokenizador utilizado.

Los modelos generativos de texto producen la respuesta token a token. El texto de entrada y el texto generado se contabilizan en tokens.

Fuente: [Jurafsky y Martin: palabras y tokens](https://web.stanford.edu/~jurafsky/slp3/2.pdf).

## 7. Determinismo y aleatoriedad

### Determinismo

Un proceso es **determinista** cuando produce el mismo resultado si se repite con las mismas entradas, el mismo estado inicial y las mismas condiciones de ejecución.

Ejemplo: una función que suma dos números devuelve siempre **13** cuando recibe **8 y 5**.

Las condiciones incluyen los datos externos que utiliza el programa: hora, archivos, respuestas de servicios o estado de una base de datos.

### Aleatoriedad

Un proceso incorpora **aleatoriedad** cuando utiliza una fuente de azar para elegir entre resultados posibles. Al repetirlo, el resultado puede cambiar, aunque también puede coincidir.

Ejemplo: un programa selecciona al azar una tarjeta entre «roja», «azul» y «verde». Si las tres tienen la misma probabilidad, cada una tiene **1/3** de posibilidades en cada selección independiente.

| Aspecto | Proceso determinista | Proceso con aleatoriedad |
| --- | --- | --- |
| Resultado | Se repite bajo las mismas condiciones. | Puede variar entre ejecuciones. |
| Ejemplo | Ordenar una lista de números. | Barajar una lista de tarjetas. |
| Uso habitual | Cálculos y transformaciones reproducibles. | Simulaciones, sorteos y exploración de alternativas. |

### Pseudoaleatoriedad y semilla

Muchos programas utilizan **generadores pseudoaleatorios**: algoritmos deterministas que producen secuencias con apariencia aleatoria.

La **semilla** (*seed*) inicializa el generador. Con el mismo generador, la misma semilla y la misma secuencia de llamadas, se puede reproducir la secuencia bajo las mismas condiciones de implementación.

Fijar la semilla una vez no hace que todas las selecciones sean iguales: cada llamada avanza el estado del generador. Reiniciarlo con la misma semilla permite repetir la secuencia.

También existen fuentes de aleatoriedad basadas en fenómenos físicos y fuentes proporcionadas por el sistema operativo.

Fuente: [Python: generadores pseudoaleatorios y semillas](https://docs.python.org/3/library/random.html).

### Relación con los modelos de lenguaje

Un modelo puede asignar probabilidades a varios tokens y utilizar diferentes reglas para elegir el siguiente.

Ejemplo: después de **«El excursionista lleva una»**, un modelo asigna estas probabilidades:

| Continuación | Probabilidad |
| --- | --- |
| «mochila» | 60 % |
| «linterna» | 30 % |
| «cantimplora» | 10 % |

- **Selección determinista:** elegir siempre el token más probable; en este caso, «mochila». Los empates requieren una regla fija.
- **Muestreo aleatorio:** seleccionar según las probabilidades. «Mochila» tiene más posibilidades, pero también pueden salir las otras opciones.

**Probabilístico** describe cómo se representan las posibilidades; **aleatorio** describe cómo se realiza la selección. Un modelo probabilístico puede utilizar una regla de selección determinista.

En un LLM, repetir una respuesta exactamente también depende del modelo, el contexto, la configuración y las condiciones de ejecución.

Fuente: [Jurafsky y Martin: probabilidades y muestreo en modelos de lenguaje](https://web.stanford.edu/~jurafsky/slp3/3.pdf).

## 8. Redes neuronales

### Origen e inspiración biológica

Las **redes neuronales artificiales** son modelos matemáticos formados por unidades conectadas, llamadas **neuronas artificiales**. Se inspiran de forma simplificada en cómo las neuronas del sistema nervioso animal reciben señales y transmiten una respuesta.

En **1943**, **Warren S. McCulloch y Walter Pitts** publicaron *A Logical Calculus of the Ideas Immanent in Nervous Activity*. Describieron neuronas idealizadas con activación de tipo «todo o nada» y redes capaces de representar operaciones lógicas.

El modelo de 1943 utilizaba conexiones fijas. Las redes modernas incorporan parámetros ajustables mediante entrenamiento.

Fuente: [McCulloch y Pitts: artículo original (1943)](https://home.csulb.edu/~cwallis/382/readings/482/mccolloch.logical.calculus.ideas.1943.pdf).

### Cómo funciona una neurona artificial

![Diagrama de una neurona artificial con entradas, pesos, sesgo, activación y salida; ejemplo de aprendizaje de una suma y ciclo de entrenamiento](assets/neurona-artificial.png)

La neurona combina sus entradas y aplica una función de activación:

**z = w₁·x₁ + w₂·x₂ + … + wₙ·xₙ + b**

**ŷ = f(z)**

| Elemento | Función |
| --- | --- |
| **Entradas (x)** | Datos numéricos recibidos por la neurona. |
| **Pesos (w)** | Multiplican cada entrada y determinan su contribución a la suma. Pueden ser positivos, negativos o cero. |
| **Sesgo (b)** | Desplaza el resultado de la suma, incluso cuando las entradas son cero. |
| **Suma ponderada (z)** | Combina los productos de entradas y pesos con el sesgo. |
| **Activación (f)** | Transforma la suma. Por ejemplo, ReLU devuelve el mayor valor entre 0 y z. |
| **Salida (ŷ)** | Predicción o señal transmitida a otras neuronas. |

Las activaciones no lineales permiten aprender relaciones complejas. Para una suma basta una activación lineal: **f(z) = z**.

Fuente: [Google: neuronas, capas y funciones de activación](https://developers.google.com/machine-learning/crash-course/neural-networks/nodes-hidden-layers).

### Ejemplo: aprender a sumar

Una neurona recibe dos números y aprende a predecir su suma:

| x₁ | x₂ | Objetivo (y) |
| --- | --- | --- |
| 1 | 4 | 5 |
| 3 | 6 | 9 |
| 6 | 2 | 8 |

Con activación lineal, una solución es **w₁ = 1**, **w₂ = 1** y **b = 0**. El entrenamiento puede aproximarse a esos valores.

Para una entrada nueva, **x₁ = 4** y **x₂ = 7**, la predicción será aproximadamente:

**ŷ ≈ 1·4 + 1·7 + 0 = 11**.

### Entrenamiento

1. **Predecir:** calcular la salida con los parámetros actuales.
2. **Medir la pérdida:** comparar la predicción **ŷ** con el objetivo **y**. Por ejemplo, utilizar el error cuadrático **(ŷ − y)²**.
3. **Ajustar:** modificar pesos y sesgo para reducir la pérdida, mediante un optimizador como el descenso de gradiente.
4. **Repetir:** procesar ejemplos durante varias pasadas por los datos y evaluar el resultado con datos nuevos.

La **tasa de aprendizaje** controla el tamaño de los ajustes. Una tasa excesiva puede desestabilizar el entrenamiento; una demasiado pequeña puede hacerlo lento.

Fuente: [Google: descenso de gradiente y ajuste de parámetros](https://developers.google.com/machine-learning/crash-course/linear-regression/gradient-descent).

### De una neurona a una red

En una red de tipo *feedforward*, las neuronas se organizan en **capa de entrada**, **capas ocultas** y **capa de salida**. Cada capa transforma la información que recibe de la anterior.

Una red con varias capas ocultas es un ejemplo de **aprendizaje profundo**. Los LLM utilizan redes neuronales de gran escala.

Fuente: [Google: organización en capas](https://developers.google.com/machine-learning/crash-course/neural-networks/nodes-hidden-layers).
