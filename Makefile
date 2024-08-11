SRC_DIR = src
TEST_DIR = tests
BUILD_DIR = build
TEST_BUILD_DIR = tbuild 
SOURCE = $(SRC_DIR)/lexer.cpp $(SRC_DIR)/parser.cpp $(SRC_DIR)/helper.cpp
DRIVER_SOURCE = $(SRC_DIR)/main.cpp
TEST_SOURCE = $(TEST_DIR)/test.cpp $(TEST_DIR)/test_parser.cpp
EXECUTABLE = snip
TEST_EXECUTABLE = testbin
DEBUG_EXECUTABLE = debug

OBJECTS = $(patsubst $(SRC_DIR)/%.cpp, $(BUILD_DIR)/%.o, $(SOURCE))
DRIVER_OBJECTS = $(patsubst $(SRC_DIR)/%.cpp, $(BUILD_DIR)/%.o, $(DRIVER_SOURCE))
TEST_OBJECTS = $(patsubst $(TEST_DIR)/%.cpp, $(TEST_BUILD_DIR)/%.o, $(TEST_SOURCE))

default: $(EXECUTABLE)

$(EXECUTABLE): $(OBJECTS) $(DRIVER_OBJECTS)
	@echo "Building the project..."
	@g++ -o $(EXECUTABLE) $(OBJECTS) $(DRIVER_OBJECTS)

$(BUILD_DIR)/%.o: $(SRC_DIR)/%.cpp
	@echo "Compiling $<..."
	@g++ -g -c $< -o $@

$(TEST_BUILD_DIR)/%.o: $(TEST_DIR)/%.cpp $(SRC_DIR)/%.cpp
	@echo "Building the project with debug symbols..."
	@echo "Compiling $< with debug symbols..."
	@g++ -g -c $< -o $@

clean:
	@echo "Cleaning up..."
	@rm -f $(EXECUTABLE) $(TEST_EXECUTABLE) $(DEBUG_EXECUTABLE) $(BUILD_DIR)/*.o

debug: $(SOURCE) $(DRIVER_SOURCE)
	@echo "Building the project with debug symbols..."
	@g++ -g -o $(DEBUG_EXECUTABLE) $(SOURCE) $(DRIVER_SOURCE) 

run: $(EXECUTABLE)
	@./$(EXECUTABLE)

test: $(TEST_OBJECTS) $(patsubst $(SRC_DIR)/%.cpp, $(TEST_BUILD_DIR)/%.o, $(SOURCE))
	@echo "Building the project with debug symbols..."
	@g++ -g -o $(TEST_EXECUTABLE) $(OBJECTS) $(TEST_OBJECTS)

.PHONY: default clean debug run test test_debug
