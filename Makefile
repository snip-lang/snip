default: build
build:
		echo "Building the project..."
		g++ -o snip src/main.cpp src/lexer.cpp
clean:
		echo "Cleaning up..."
		rm -f snip
debug:
		echo "Building the project with debug symbols..."
		g++ -g -o snip src/main.cpp src/lexer.cpp
run:
		g++ -o snip src/main.cpp src/lexer.cpp && ./snip
test:
		echo "Running tests..."
		g++ -o snip tests/main.cpp src/lexer.cpp && ./snip
