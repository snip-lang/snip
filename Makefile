# Variables
SRC_DIR = src
BUILD_DIR = build
SOURCE = $(SRC_DIR)/lexer.cpp $(SRC_DIR)/parser.cpp $(SRC_DIR)/helper.cpp
DRIVER_SOURCE = $(SRC_DIR)/main.cpp
TESTS = tests/test.cpp tests/test_parser.cpp
EXECUTABLE = snip
TEST_EXECUTABLE = snip_tests
DEBUG_EXECUTABLE = debug

# Object files
OBJECTS = $(patsubst $(SRC_DIR)/%.cpp, $(BUILD_DIR)/%.o, $(SOURCE))
DRIVER_OBJECTS = $(patsubst $(SRC_DIR)/%.cpp, $(BUILD_DIR)/%.o, $(DRIVER_SOURCE))
TEST_OBJECTS = $(patsubst %.cpp, $(BUILD_DIR)/%.o, $(TESTS))

# Default target
default: $(EXECUTABLE)

# Build executable
$(EXECUTABLE): $(OBJECTS) $(DRIVER_OBJECTS)
	@echo "Building the project..."
	@g++ -o $(EXECUTABLE) $(OBJECTS) $(DRIVER_OBJECTS)

# Build object files from source files
$(BUILD_DIR)/%.o: $(SRC_DIR)/%.cpp
	@mkdir -p $(BUILD_DIR)
	@echo "Compiling $<..."
	@g++ -c $< -o $@

# Build object files from test files
$(BUILD_DIR)/%.o: %.cpp
	@mkdir -p $(BUILD_DIR)
	@echo "Compiling $<..."
	@g++ -c $< -o $@

# Clean build directory
clean:
	@echo "Cleaning up..."
	@rm -f $(EXECUTABLE) $(TEST_EXECUTABLE) $(DEBUG_EXECUTABLE) $(BUILD_DIR)/*.o

# Build with debug symbols
debug: $(OBJECTS) $(DRIVER_OBJECTS)
	@echo "Building the project with debug symbols..."
	@g++ -g -o $(DEBUG_EXECUTABLE) $(OBJECTS) $(DRIVER_OBJECTS)

# Run the project
run: $(EXECUTABLE)
	@./$(EXECUTABLE)

# Run tests
test: $(TEST_OBJECTS) $(OBJECTS)
	@echo "Running tests..."
	@g++ -o $(TEST_EXECUTABLE) $(OBJECTS) $(TEST_OBJECTS)
	@./$(TEST_EXECUTABLE)

# Run tests with debug symbols
test_debug: $(TEST_OBJECTS) $(OBJECTS)
	@echo "Running tests with debug symbols..."
	@g++ -g -o $(TEST_EXECUTABLE) $(OBJECTS) $(TEST_OBJECTS)
	@./$(TEST_EXECUTABLE)

# Phony targets
.PHONY: default clean debug run test test_debug
