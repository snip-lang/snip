SRC_DIR = src
BUILD_DIR = build
SOURCE = $(SRC_DIR)/lexer.cpp $(SRC_DIR)/parser.cpp $(SRC_DIR)/helper.cpp
DRIVER_SOURCE = $(SRC_DIR)/main.cpp
TESTS = tests/test.cpp tests/test_parser.cpp
EXECUTABLE = snip
TEST_EXECUTABLE = snip_tests
DEBUG_EXECUTABLE = debug

OBJECTS = $(patsubst $(SRC_DIR)/%.cpp, $(BUILD_DIR)/%.o, $(SOURCE))
DRIVER_OBJECTS = $(patsubst $(SRC_DIR)/%.cpp, $(BUILD_DIR)/%.o, $(DRIVER_SOURCE))
TEST_OBJECTS = $(patsubst %.cpp, $(BUILD_DIR)/%.o, $(TESTS))

default: $(EXECUTABLE)

$(EXECUTABLE): $(OBJECTS) $(DRIVER_OBJECTS)
	@echo "Building the project..."
	@g++ -o $(EXECUTABLE) $(OBJECTS) $(DRIVER_OBJECTS)

$(BUILD_DIR)/%.o: $(SRC_DIR)/%.cpp
	@echo "Compiling $<..."
	@g++ -c $< -o $@

$(BUILD_DIR)/%.o: %.cpp
	@echo "Compiling $<..."
	@g++ -c $< -o $@

clean:
	@echo "Cleaning up..."
	@rm -f $(EXECUTABLE) $(TEST_EXECUTABLE) $(DEBUG_EXECUTABLE) $(BUILD_DIR)/*.o

debug: $(OBJECTS) $(DRIVER_OBJECTS)
	@echo "Building the project with debug symbols..."
	@g++ -g -o $(DEBUG_EXECUTABLE) $(OBJECTS) $(DRIVER_OBJECTS)

run: $(EXECUTABLE)
	@./$(EXECUTABLE)

test: $(TEST_OBJECTS) $(OBJECTS)
	@echo "Running tests..."
	@g++ -o $(TEST_EXECUTABLE) $(OBJECTS) $(TEST_OBJECTS)
	@./$(TEST_EXECUTABLE)

test_debug: $(TEST_OBJECTS) $(OBJECTS)
	@echo "Running tests with debug symbols..."
	@g++ -g -o $(TEST_EXECUTABLE) $(OBJECTS) $(TEST_OBJECTS)
	@./$(TEST_EXECUTABLE)

.PHONY: default clean debug run test test_debug
