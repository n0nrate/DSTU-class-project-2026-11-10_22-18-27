# Прогресс MetroPlatform

## Текущее состояние

- Текущий этап: четвёртый урок «Прыжок II» выполнен.
- Следующий урок: №5 «Визуал персонажа», ещё не начат.
- Unity: `6000.3.28f1` (Unity 6.3 LTS).
- Основная сцена: `Assets/Scenes/SampleScene.unity`; добавлена в Build Settings.
- Последний учебный коммит: `9e1730b` — «Добавить базовую сцену и PlayerMovement из первого урока».

## Что уже сделано в первом уроке

- Создана базовая сцена с объектами `Background`, `Floor`, `Wall Left`, `Wall Center`, `Wall Right`, `Player`, `Main Camera` и `Global Light 2D`.
- Созданы префабы `Assets/Prefabs/Player.prefab` и `Assets/Prefabs/Wall.prefab`.
- У `Player` есть `Rigidbody2D`, `BoxCollider2D` и компонент `PlayerMovement`.
- У `Wall` есть `BoxCollider2D`.
- Создан `Assets/Scripts/PlayerMovement.cs`.
- Поле `_rigidbody` связано с `Rigidbody2D` в префабе и сцене; в `Start()` ссылка также получается через `GetComponent<Rigidbody2D>()`.
- `Update()` пока пустой: фактическое управление персонажем ещё не реализовано.

## Проверка от 2026-11-10

- В открытом Unity Editor Console показывает 0 сообщений, 0 предупреждений и 0 ошибок.
- Сцена `SampleScene` открыта и имеет несохранённые изменения (`*` в заголовке Unity); они не сохранялись во время аудита.
- Unity CLI не установлен или недоступен в `PATH`, поэтому Console проверена непосредственно через интерфейс Unity.
- В рабочем дереве Git до документирования уже находились незакоммиченные изменения настроек Unity, удаление `Assets/Editor/HubForceResolve.cs` и новые файлы Package Manager. Эти изменения не исправлялись и не откатывались.

## Урок 2 — движение персонажа

- Создан `Assets/Scripts/PlayerInput.cs`.
- `PlayerInput.Update()` читает горизонтальную ось через `Input.GetAxisRaw("Horizontal")`.
- `PlayerMovement` перенесён в пространство имён `MetroPlatform`.
- В `PlayerMovement` добавлена скорость ходьбы `6`.
- Ссылки на `Rigidbody2D` и `PlayerInput` автоматически находятся в `Awake()`, если не назначены в Inspector.
- Горизонтальная скорость вычисляется отдельным методом `FixedMove()`.
- В `FixedUpdate()` результат записывается в `Rigidbody2D.linearVelocity`.
- На объект `Player` сцены добавлен учебный компонент `Player Input (Script)`.
- `Rigidbody2D` объекта `Player`: Gravity Scale `8`, Collision Detection `Continuous`, Interpolate `Interpolate`, Freeze Rotation Z включён.
- Для совместимости с кодом урока `Active Input Handling` переключён на `Input Manager (Old)`. Unity 6 предупреждает, что этот ввод устарел; на данном этапе это ожидаемо.
- В Play Mode проверено движение вправо: координата X изменилась с `2.8` до `3.8744`.
- После повторной проверки Console: 0 ошибок и 0 предупреждений.

## Визуальное оформление текущей сцены

- Без импорта изображений и без отдельной папки со спрайтами использован встроенный примитив Unity `Square`.
- `Square` назначен существующим объектам `Background`, `Floor`, `Wall Left`, `Wall Center`, `Wall Right` и `Player`.
- Сохранены ранее заданные цвета и размеры: синий фон, белый пол, голубые стены и красный игрок.
- Вспомогательный объект `Square` оставлен в Hierarchy, но его `Sprite Renderer` выключен, поэтому лишний белый квадрат не отображается.
- После сохранения Console: 0 ошибок и 0 предупреждений.

## Урок 3 — прыжок I

- `PlayerInput` считывает одноразовое нажатие кнопки `Jump` через `Input.GetButtonDown("Jump")`.
- В `PlayerMovement` добавлены сила прыжка `9` и длительность `0.15` секунды.
- Запрос прыжка обрабатывается в `Update()`, а скорость прыжка задаётся в `FixedUpdate()` через отдельный метод `FixedJump()`.
- Таймер `_jumpCounter` ограничивает время, в течение которого к вертикальной скорости применяется `_jumpForce`.
- В `Player` создан дочерний объект `GroundCheck`.
- `Grounded()` пускает короткий `Physics2D.Raycast` вниз и разрешает прыжок только при обнаружении коллайдера.
- Добавлен `Debug.DrawRay`, чтобы видеть направление проверки в Scene View.
- Созданы слои `ObjectsInteractive`, `ObjectsStatic` и `Player`; игрок находится на `Player`, пол и стены — на `ObjectsStatic`.
- Маска `_jumpLayerMask` проверяет `ObjectsInteractive` и `ObjectsStatic`, но не самого игрока — это исправление из конца урока.
- В Play Mode временная диагностика подтвердила: нажатие `Jump` считывается, `Grounded()` возвращает `true`. Временное сообщение из кода удалено.

## Урок 4 — прыжок II

- На дочерний объект `GroundCheck` добавлен `BoxCollider2D` размером `1 x 0.1`; он работает как триггер и не толкает поверхности.
- Точечный `Physics2D.Raycast` заменён на `Physics2D.BoxCast`, который проверяет землю по ширине коллайдера `GroundCheck`.
- В `Grounded()` сохраняется найденный коллайдер поверхности в поле `_groundHit`, чтобы результат был виден в Inspector.
- Добавлен буфер ввода прыжка длительностью `0.15` секунды: раннее нажатие пробела не теряется непосредственно перед приземлением.
- Добавлен coyote time длительностью `0.15` секунды: после схода с края платформы остаётся короткое время для прыжка.
- После успешного начала прыжка счётчики буфера и coyote time обнуляются.
- Unity автоматически перечитал изменённые файлы; в журнале после обновления проекта нет ошибок компиляции `PlayerMovement.cs`.
- Отдельная сборка через `dotnet build` не использована для проверки: установленный .NET SDK не получил доступ к каталогу локального Windows SDK. Это ограничение внешней проверки, а не ошибка C#-кода Unity.
- Проверка в открытом Unity Editor от 2026-11-11: `GroundCheck` имеет включённый триггер `BoxCollider2D` размером `1 x 0.1`; в Play Mode поля `Can Jump` и `Grounded` были активны, а `Ground Hit` указывал на `Floor`.
- Обычный прыжок проверен в Play Mode: после нажатия пробела игрок оказался над платформой. После остановки игры Console показывает 0 сообщений, 0 предупреждений и 0 ошибок.

## Что проверить в Unity

1. Открыта сцена `Assets/Scenes/SampleScene.unity`.
2. В Hierarchy присутствуют перечисленные объекты сцены.
3. На `Player` находятся `Rigidbody2D`, `BoxCollider2D` и `PlayerMovement`.
4. На `Wall` находится `BoxCollider2D`.
5. На `Player` добавлен `Player Input (Script)`.
6. У `Rigidbody2D` стоят Gravity Scale `8`, Continuous, Interpolate и Freeze Rotation Z.
7. В Play Mode клавиши A/D или стрелки влево/вправо двигают персонажа.
8. У `Player` есть дочерний `GroundCheck`, а в `PlayerMovement` назначены `Jump Layer Mask` и `Ground Check T`.
9. В Play Mode пробел запускает прыжок только с поверхности.
10. У `GroundCheck` есть `BoxCollider2D`, а в `PlayerMovement` назначено поле `Ground Check Coll`.
11. Если нажать пробел совсем немного до приземления, персонаж прыгает сразу после касания пола — это буфер ввода.
12. Если сойти с края и почти сразу нажать пробел, прыжок ещё срабатывает — это coyote time.
13. Console не содержит ошибок C# и предупреждений проекта.

## Следующий шаг

Не переходить к уроку 5, пока пользователь не попросит продолжить.
