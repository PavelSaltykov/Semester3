# SPbU Homework and Tests

Semester №3 |
:-: |
[Homework №1](#homework-1) |
[Homework №2](#homework-2) |
[Homework №3](#homework-3) |

## Homework №1
Одна из самых полезных и вместе с тем хорошо параллелящихся задач — умножение матриц. Это часто используется не только в научных расчётах, но и при практически любой работе с 
графикой, особенно трёхмерной. Фактически, современные видеокарты — это специализированные вычислители, умеющие умножать матрицы (в частности, вектора на матрицы) очень 
эффективно за счёт большого количества вычислительных узлов (до сотен в современных выделенных видеокартах). Естественно, параллельно. Вам надо попробовать поумножать 
матрицы с помощью обычного многоядерного процессора.

Требуется реализовать параллельное умножение для плотных матриц. На входе программа получает два файла с матрицами (не обязательно квадратными), на выходе должен получиться файл,
содержащий матрицу — их произведение. Сравнить скорость работы с последовательным вариантом в зависимости от размеров матриц. Попробовать получить возможно большее ускорение.

*Обратите внимание, что распараллеливание имеет смысл только на достаточно больших данных, так что требуется также уметь генерировать большие тестовые данные и найти такие 
размеры данных, при которых различия в скорости работы будут заметны и значительны.* 

[Solution](https://github.com/PavelSaltykov/Semester3/tree/master/Homeworks/Task1)

## Homework №2
Реализовать следующий интерфейс, представляющий ленивое вычисление:

```
public interface ILazy<T> {
        T Get();
}
```

Объект Lazy создаётся на основе вычисления (представляемого объектом Func\<T>)

- Первый вызов Get() вызывает вычисление и возвращает результат
- Повторные вызовы Get() возвращают тот же объект, что и первый вызов
- Вычисление должно запускаться не более одного раза

Создавать объекты надо не вручную, а с помощью класса LazyFactory, который должен иметь два метода с сигнатурами наподобие
```
public static Lazy<T> Create...Lazy<T>(Func<T> supplier)
```
возвращающих две разные реализации ILazy\<T>:

- Простая версия с гарантией корректной работы в однопоточном режиме (без синхронизации)
- Гарантия корректной работы в многопоточном режиме
  - При этом она должна по возможности минимизировать число необходимых синхронизаций (если значение уже вычислено, не должно быть блокировок)

- supplier вправе вернуть null
- Библиотечным Lazy пользоваться, естественно, нельзя

Нужно:

- CI, на котором проходят ваши тесты
- Тесты
  - Однопоточные, на разные хорошие и плохие случаи
  - Многопоточные, на наличие гонок
  
[Solution](https://github.com/PavelSaltykov/Semester3/tree/master/Homeworks/Task2)

## Homework №3
Реализовать простой пул задач (наподобие https://docs.microsoft.com/en-us/dotnet/api/system.threading.threadpool?view=netframework-4.8 + 
https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskfactory?view=netframework-4.8) с фиксированным числом потоков (число задается в конструкторе)

- При создании объекта MyThreadPool в нем должно начать работу n потоков
- У каждого потока есть два состояния: ожидание задачи / выполнение задачи
- Задача — вычисление некоторого значения, описывается в виде Func\<TResult>
- При добавлении задачи, если в пуле есть ожидающий поток, то он должен приступить к ее исполнению. Иначе задача будет ожидать исполнения, пока не освободится какой-нибудь поток
- Задачи, принятые к исполнению, представлены в виде объектов интерфейса IMyTask\<TResult>
- Метод Shutdown должен завершить работу потоков. Завершение работы коллаборативное, с использованием CancellationToken — уже запущенные задачи не прерываются, но новые задачи не принимаются на исполнение потоками из пула. 
  - Возможны два варианта решения — дать всем задачам, которые уже попали в очередь, досчитаться, либо выбросить исключение во все ожидающие завершения задачи потоки
  - Shutdown не должен возвращать управление, пока все потоки не остановились
- IMyTask 
  - Свойство IsCompleted возвращает true, если задача выполнена 
  - Свойство Result возвращает результат выполнения задачи   
  - В случае, если соответствующая задаче функция завершилась с исключением, этот метод должен завершиться с исключением AggregateException, 
  содержащим внутри себя исключение, вызвавшее проблему
  - Если результат еще не вычислен, метод ожидает его и возвращает полученное значение, блокируя вызвавший его поток
  - Метод ContinueWith — принимает объект типа Func\<TResult, TNewResult>, который может быть применен к результату данной задачи X и возвращает новую задачу Y, 
  принятую к исполнению
  - Новая задача будет исполнена не ранее, чем завершится исходная
  - В качестве аргумента объекту Func будет передан результат исходной задачи, и все Y должны исполняться на общих основаниях (т.е. должны разделяться между потоками пула)
  - Метод ContinueWith может быть вызван несколько раз
  - Метод ContinueWith не должен блокировать работу потока, если результат задачи X ещё не вычислен
  - ContinueWith должен быть согласован с Shutdown — принятая как ContinueWith задача должна либо досчитаться, либо бросить исключение ожидающему её потоку.

При этом:

- В данной работе запрещено использование TPL, PLINQ и библиотечных классов Task и ThreadPool.
- Все интерфейсные методы должны быть потокобезопасны
- Для каждого базового сценария использования должен быть написан несложный тест
- Также должен быть написан тест, проверяющий, что в пуле действительно не менее n потоков

Подсказка: задачи могут быть разных типов (например, можно `var myTask = myThreadPool.Submit(() => 2 * 2).ContinueWith(x => x.ToString());`).
Хранить такие задачи в очереди можно, обернув их в Action.