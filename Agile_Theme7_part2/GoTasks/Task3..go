// Дублирование видимых данных

package main

import (
	"fmt"
	"math/rand"
	"sync"
	"time"
)

// ЧАСТЬ 1: ПЛОХОЕ РЕШЕНИЕ (с дублированием)

// BadCurrencyService - сервис с дублированием данных
type BadCurrencyService struct {
	// Данные дублируются для каждого компонента
	mainScreenRate float64
	widgetRate     float64
	lastUpdate     time.Time
}

// NewBadCurrencyService - создает плохой сервис
func NewBadCurrencyService() *BadCurrencyService {
	bcs := &BadCurrencyService{
		mainScreenRate: 50.0,
		widgetRate:     50.0,
		lastUpdate:     time.Now(),
	}

	// Запускаем обновление
	go bcs.startUpdating()
	return bcs
}

// startUpdating - дублирование логики обновления
func (bcs *BadCurrencyService) startUpdating() {
	ticker := time.NewTicker(1 * time.Second)
	for range ticker.C {
		newRate := 50.0 + rand.Float64()*10

		// ПРОБЛЕМА 1: Обновляем ДВА раза
		bcs.mainScreenRate = newRate
		bcs.widgetRate = newRate
		bcs.lastUpdate = time.Now()

		// ПРОБЛЕМА 2: Логируем ДВА раза
		fmt.Printf("[BAD] MAIN Screen Rate: %.2f\n", bcs.mainScreenRate)
		fmt.Printf("[BAD] WIDGET Rate: %.2f\n", bcs.widgetRate)
	}
}

// GetMainScreenRate - геттер для главного экрана
func (bcs *BadCurrencyService) GetMainScreenRate() float64 {
	// ПРОБЛЕМА 3: Дублирование проверки актуальности
	if time.Since(bcs.lastUpdate) > 5*time.Second {
		fmt.Println("[BAD] WARNING: Rate might be stale (main screen)")
	}
	return bcs.mainScreenRate
}

// GetWidgetRate - геттер для виджета (дублирование логики)
func (bcs *BadCurrencyService) GetWidgetRate() float64 {
	// Та же самая проверка скопирована
	if time.Since(bcs.lastUpdate) > 5*time.Second {
		fmt.Println("[BAD] WARNING: Rate might be stale (widget)")
	}
	return bcs.widgetRate
}

// ForceUpdate - принудительное обновление
func (bcs *BadCurrencyService) ForceUpdate(newRate float64) {
	// ПРОБЛЕМА 4: Обновляем в ДВУХ местах
	bcs.mainScreenRate = newRate
	bcs.widgetRate = newRate
	fmt.Printf("[BAD] Forced update both: %.2f\n", newRate)
}

// runBadSolution - запуск плохого решения
func runBadSolution() {
	fmt.Println("ПЛОХОЕ РЕШЕНИЕ (с дублированием данных)")

	service := NewBadCurrencyService()

	// Эмуляция компонентов, которые читают свои копии данных
	go func() {
		for {
			time.Sleep(2 * time.Second)
			rate := service.GetMainScreenRate()
			fmt.Printf("[BAD] 🏠 MAIN DISPLAY: %.2f\n", rate)
		}
	}()

	go func() {
		for {
			time.Sleep(3 * time.Second)
			rate := service.GetWidgetRate()
			fmt.Printf("[BAD] 📱 WIDGET: %.2f\n", rate)
		}
	}()

	// Демонстрация проблемы через 5 секунд
	go func() {
		time.Sleep(5 * time.Second)
		fmt.Println("\n[BAD] 🔄 Принудительное обновление...")
		service.ForceUpdate(99.99)
	}()

	time.Sleep(10 * time.Second)
	fmt.Println("\n[BAD] ❌ ПРОБЛЕМЫ ЭТОГО ПОДХОДА:")
	fmt.Println("   1. Дублирование данных в памяти")
	fmt.Println("   2. Дублирование логики обновления")
	fmt.Println("   3. Риск рассинхрона данных")
	fmt.Println("   4. Сложность поддержки и расширения")
	fmt.Println("   5. При добавлении нового компонента нужно копировать код")
}

// ЧАСТЬ 2: ХОРОШЕЕ РЕШЕНИЕ (без дублирования)

// CurrencyData - единая структура для хранения данных (один источник правды)
type CurrencyData struct {
	Rate      float64
	UpdatedAt time.Time
}

// GoodCurrencyService - сервис с единственным источником данных
type GoodCurrencyService struct {
	mu          sync.RWMutex
	current     CurrencyData
	subscribers []chan CurrencyData // Паттерн Наблюдатель
}

// NewGoodCurrencyService - создает хороший сервис
func NewGoodCurrencyService() *GoodCurrencyService {
	gcs := &GoodCurrencyService{
		current: CurrencyData{
			Rate:      50.0,
			UpdatedAt: time.Now(),
		},
		subscribers: make([]chan CurrencyData, 0),
	}

	go gcs.startUpdating()
	return gcs
}

// startUpdating - ЕДИНАЯ логика обновления (в одном месте!)
func (gcs *GoodCurrencyService) startUpdating() {
	ticker := time.NewTicker(1 * time.Second)
	for range ticker.C {
		newRate := 50.0 + rand.Float64()*10

		// Обновляем ТОЛЬКО ОДИН раз
		gcs.mu.Lock()
		gcs.current = CurrencyData{
			Rate:      newRate,
			UpdatedAt: time.Now(),
		}
		currentCopy := gcs.current
		gcs.mu.Unlock()

		// Единое логирование (один раз!)
		fmt.Printf("[GOOD] 💱 Rate updated: %.2f\n", currentCopy.Rate)

		// Оповещаем всех подписчиков
		gcs.notifySubscribers(currentCopy)
	}
}

// GetCurrentRate - ЕДИНЫЙ метод получения курса (один для всех!)
func (gcs *GoodCurrencyService) GetCurrentRate() CurrencyData {
	gcs.mu.RLock()
	defer gcs.mu.RUnlock()

	rate := gcs.current

	// Единая проверка актуальности (в одном месте!)
	if time.Since(rate.UpdatedAt) > 5*time.Second {
		fmt.Println("[GOOD] ⚠️ WARNING: Rate might be stale")
	}

	return rate
}

// ForceUpdate - принудительное обновление (в одном месте!)
func (gcs *GoodCurrencyService) ForceUpdate(newRate float64) {
	gcs.mu.Lock()
	defer gcs.mu.Unlock()

	gcs.current = CurrencyData{
		Rate:      newRate,
		UpdatedAt: time.Now(),
	}
	fmt.Printf("[GOOD] 🔄 Force update: %.2f\n", newRate)
}

// Subscribe - подписка на обновления
func (gcs *GoodCurrencyService) Subscribe() <-chan CurrencyData {
	ch := make(chan CurrencyData, 10)
	gcs.mu.Lock()
	gcs.subscribers = append(gcs.subscribers, ch)
	gcs.mu.Unlock()
	return ch
}

// notifySubscribers - уведомление всех подписчиков
func (gcs *GoodCurrencyService) notifySubscribers(data CurrencyData) {
	gcs.mu.RLock()
	subscribers := make([]chan CurrencyData, len(gcs.subscribers))
	copy(subscribers, gcs.subscribers)
	gcs.mu.RUnlock()

	for _, sub := range subscribers {
		select {
		case sub <- data:
		default:
			// Пропускаем, если канал заполнен
		}
	}
}

// MainDisplay - компонент главного экрана
type MainDisplay struct {
	service *GoodCurrencyService
}

func NewMainDisplay(service *GoodCurrencyService) *MainDisplay {
	return &MainDisplay{service: service}
}

func (md *MainDisplay) Start() {
	ticker := time.NewTicker(2 * time.Second)
	for range ticker.C {
		rate := md.service.GetCurrentRate()
		fmt.Printf("[GOOD] 🏠 MAIN DISPLAY: USD/RUB = %.2f (updated: %s)\n",
			rate.Rate, rate.UpdatedAt.Format("15:04:05"))
	}
}

// Widget - компонент виджета
type Widget struct {
	service *GoodCurrencyService
}

func NewWidget(service *GoodCurrencyService) *Widget {
	return &Widget{service: service}
}

func (w *Widget) Start() {
	ticker := time.NewTicker(3 * time.Second)
	for range ticker.C {
		rate := w.service.GetCurrentRate()
		fmt.Printf("[GOOD] 📱 WIDGET: %.2f ₽\n", rate.Rate)
	}
}

// RealtimeMonitor - монитор реального времени
type RealtimeMonitor struct {
	updates <-chan CurrencyData
}

func NewRealtimeMonitor(service *GoodCurrencyService) *RealtimeMonitor {
	return &RealtimeMonitor{
		updates: service.Subscribe(),
	}
}

func (rm *RealtimeMonitor) Start() {
	for update := range rm.updates {
		fmt.Printf("[GOOD] 🔔 REALTIME: Rate changed to %.2f at %s\n",
			update.Rate, update.UpdatedAt.Format("15:04:05"))
	}
}

// runGoodSolution - запуск хорошего решения
func runGoodSolution() {
	fmt.Println("ХОРОШЕЕ РЕШЕНИЕ (без дублирования данных)")

	// Создаем ОДИН сервис (единый источник правды)
	service := NewGoodCurrencyService()

	// Все компоненты используют ОДИН источник данных
	mainDisplay := NewMainDisplay(service)
	widget := NewWidget(service)
	monitor := NewRealtimeMonitor(service)

	// Запускаем компоненты
	go mainDisplay.Start()
	go widget.Start()
	go monitor.Start()

	// Демонстрация ForceUpdate через 5 секунд
	go func() {
		time.Sleep(5 * time.Second)
		fmt.Println("\n[GOOD] 🔄 Принудительное обновление...")
		service.ForceUpdate(99.99)
	}()

	time.Sleep(10 * time.Second)

	fmt.Println("\n[GOOD] ✅ ПРЕИМУЩЕСТВА ЭТОГО ПОДХОДА:")
	fmt.Println("   1. Нет дублирования данных (один источник правды)")
	fmt.Println("   2. Нет дублирования логики")
	fmt.Println("   3. Данные всегда консистентны")
	fmt.Println("   4. Легко добавлять новые компоненты")
	fmt.Println("   5. Простое тестирование и поддержка")
	fmt.Println("   6. Паттерн Наблюдатель для реактивности")
}

// ============================================
// ОСНОВНАЯ ФУНКЦИЯ
// ============================================

func main() {
	// Запускаем плохое решение
	runBadSolution()

	// Пауза перед хорошим решением
	fmt.Println("НАЖМИТЕ ENTER ДЛЯ ХОРОШЕГО РЕШЕНИЯ...")
	fmt.Scanln()

	// Запускаем хорошее решение
	runGoodSolution()

	fmt.Println("ВЫВОД:")
	fmt.Println("Плохое решение ❌ - дублирование данных и логики")
	fmt.Println("Хорошее решение ✅ - Single Source of Truth + Observer Pattern")
	fmt.Println("\nНажмите ENTER для выхода...")
	fmt.Scanln()
}
