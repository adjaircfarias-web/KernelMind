-- =====================================================
-- KernelMind Database Initialization
-- PostgreSQL with pgvector extension
-- =====================================================

-- Enable pgvector extension
CREATE EXTENSION IF NOT EXISTS vector;

-- =====================================================
-- Create Database
-- =====================================================
-- Database is already created via POSTGRES_DB env variable

-- =====================================================
-- Create Schemas
-- =====================================================
CREATE SCHEMA IF NOT EXISTS kernelmind;

-- =====================================================
-- Create Tables
-- =====================================================

-- Pizzas table with vector embeddings for semantic search
CREATE TABLE IF NOT EXISTS kernelmind.pizzas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL,
    category VARCHAR(50),
    ingredients TEXT[],
    embedding VECTOR(768),  -- nomic-embed-text produces 768 dimensions
    is_available BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create index for vector similarity search
CREATE INDEX IF NOT EXISTS idx_pizzas_embedding 
ON kernelmind.pizzas 
USING ivfflat (embedding vector_cosine_ops)
WITH (lists = 100);

-- Customers table
CREATE TABLE IF NOT EXISTS kernelmind.customers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(200) NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(200),
    address TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Orders table
CREATE TABLE IF NOT EXISTS kernelmind.orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID REFERENCES kernelmind.customers(id),
    status VARCHAR(50) DEFAULT 'pending',
    total_amount DECIMAL(10, 2) DEFAULT 0,
    delivery_address TEXT,
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Order items table
CREATE TABLE IF NOT EXISTS kernelmind.order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID REFERENCES kernelmind.orders(id) ON DELETE CASCADE,
    pizza_id UUID REFERENCES kernelmind.pizzas(id),
    quantity INTEGER NOT NULL DEFAULT 1,
    unit_price DECIMAL(10, 2) NOT NULL,
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Chat sessions table
CREATE TABLE IF NOT EXISTS kernelmind.chat_sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID REFERENCES kernelmind.customers(id),
    session_token VARCHAR(255) UNIQUE,
    context JSONB DEFAULT '{}',
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    last_activity_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Chat messages table
CREATE TABLE IF NOT EXISTS kernelmind.chat_messages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    session_id UUID REFERENCES kernelmind.chat_sessions(id) ON DELETE CASCADE,
    role VARCHAR(50) NOT NULL,  -- 'user', 'assistant', 'system'
    content TEXT NOT NULL,
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Vector store for RAG documents
CREATE TABLE IF NOT EXISTS kernelmind.vector_documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    document_type VARCHAR(50) NOT NULL,  -- 'faq', 'policy', 'menu_info', etc.
    title VARCHAR(255),
    content TEXT NOT NULL,
    embedding VECTOR(768),
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create index for vector similarity search on documents
CREATE INDEX IF NOT EXISTS idx_vector_documents_embedding 
ON kernelmind.vector_documents 
USING ivfflat (embedding vector_cosine_ops)
WITH (lists = 100);

-- =====================================================
-- Create Functions
-- =====================================================

-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION kernelmind.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers for updated_at
CREATE TRIGGER update_pizzas_updated_at 
    BEFORE UPDATE ON kernelmind.pizzas 
    FOR EACH ROW EXECUTE FUNCTION kernelmind.update_updated_at_column();

CREATE TRIGGER update_customers_updated_at 
    BEFORE UPDATE ON kernelmind.customers 
    FOR EACH ROW EXECUTE FUNCTION kernelmind.update_updated_at_column();

CREATE TRIGGER update_orders_updated_at 
    BEFORE UPDATE ON kernelmind.orders 
    FOR EACH ROW EXECUTE FUNCTION kernelmind.update_updated_at_column();

CREATE TRIGGER update_chat_sessions_updated_at 
    BEFORE UPDATE ON kernelmind.chat_sessions 
    FOR EACH ROW EXECUTE FUNCTION kernelmind.update_updated_at_column();

-- Function to search pizzas by similarity
CREATE OR REPLACE FUNCTION kernelmind.search_pizzas(
    query_embedding VECTOR(768),
    similarity_threshold FLOAT DEFAULT 0.7,
    max_results INTEGER DEFAULT 10
)
RETURNS TABLE (
    id UUID,
    name VARCHAR,
    description TEXT,
    price DECIMAL,
    category VARCHAR,
    ingredients TEXT[],
    similarity FLOAT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        p.id,
        p.name,
        p.description,
        p.price,
        p.category,
        p.ingredients,
        1 - (p.embedding <=> query_embedding) AS similarity
    FROM kernelmind.pizzas p
    WHERE p.is_available = true
    AND 1 - (p.embedding <=> query_embedding) > similarity_threshold
    ORDER BY p.embedding <=> query_embedding
    LIMIT max_results;
END;
$$ LANGUAGE plpgsql;

-- Function to search documents by similarity
CREATE OR REPLACE FUNCTION kernelmind.search_documents(
    query_embedding VECTOR(768),
    doc_type VARCHAR DEFAULT NULL,
    similarity_threshold FLOAT DEFAULT 0.7,
    max_results INTEGER DEFAULT 5
)
RETURNS TABLE (
    id UUID,
    document_type VARCHAR,
    title VARCHAR,
    content TEXT,
    metadata JSONB,
    similarity FLOAT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        d.id,
        d.document_type,
        d.title,
        d.content,
        d.metadata,
        1 - (d.embedding <=> query_embedding) AS similarity
    FROM kernelmind.vector_documents d
    WHERE (doc_type IS NULL OR d.document_type = doc_type)
    AND 1 - (d.embedding <=> query_embedding) > similarity_threshold
    ORDER BY d.embedding <=> query_embedding
    LIMIT max_results;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- Seed Data - Sample Pizzas
-- =====================================================

INSERT INTO kernelmind.pizzas (name, description, price, category, ingredients) VALUES
('Margherita', 'Classic Italian pizza with tomato sauce, mozzarella, and fresh basil', 35.00, 'Traditional', ARRAY['tomato sauce', 'mozzarella', 'fresh basil']),
('Pepperoni', 'Spicy pepperoni with mozzarella on tomato sauce', 42.00, 'Traditional', ARRAY['tomato sauce', 'mozzarella', 'pepperoni']),
('Quattro Formaggi', 'Four cheese blend: mozzarella, gorgonzola, parmesan, and provolone', 48.00, 'Specialty', ARRAY['mozzarella', 'gorgonzola', 'parmesan', 'provolone']),
('Calabresa', 'Brazilian-style calabresa sausage with onions', 40.00, 'Brazilian', ARRAY['tomato sauce', 'mozzarella', 'calabresa sausage', 'onions']),
('Frango com Catupiry', 'Shredded chicken with creamy catupiry cheese', 45.00, 'Brazilian', ARRAY['mozzarella', 'shredded chicken', 'catupiry']),
('Portuguesa', 'Ham, eggs, onions, and olives', 43.00, 'Brazilian', ARRAY['tomato sauce', 'mozzarella', 'ham', 'eggs', 'onions', 'olives']),
('Vegetariana', 'Bell peppers, mushrooms, onions, tomatoes, and olives', 38.00, 'Vegetarian', ARRAY['tomato sauce', 'mozzarella', 'bell peppers', 'mushrooms', 'onions', 'tomatoes', 'olives']),
('Supreme', 'Pepperoni, sausage, bell peppers, onions, and mushrooms', 50.00, 'Premium', ARRAY['tomato sauce', 'mozzarella', 'pepperoni', 'sausage', 'bell peppers', 'onions', 'mushrooms']);

-- =====================================================
-- Seed Data - FAQ Documents for RAG
-- =====================================================

INSERT INTO kernelmind.vector_documents (document_type, title, content) VALUES
('faq', 'Horário de Funcionamento', 'Estamos abertos todos os dias das 18:00 às 23:00. Nos finais de semana, abrimos uma hora mais cedo, às 17:00.'),
('faq', 'Tempo de Entrega', 'O tempo médio de entrega é de 30 a 45 minutos, dependendo da sua localização e do volume de pedidos.'),
('faq', 'Formas de Pagamento', 'Aceitamos pagamento em dinheiro, cartões de crédito e débito, Pix, e carteiras digitais como Apple Pay e Google Pay.'),
('faq', 'Taxa de Entrega', 'A taxa de entrega varia de R$ 3,00 a R$ 8,00 dependendo da distância. Consulte seu CEP para saber o valor exato.'),
('policy', 'Cancelamento', 'Pedidos podem ser cancelados em até 10 minutos após a confirmação. Após esse prazo, entre em contato pelo telefone.'),
('menu_info', 'Tamanhos', 'Oferecemos pizzas nos tamanhos: Pequena (25cm - 4 fatias), Média (30cm - 6 fatias), Grande (35cm - 8 fatias), e Família (40cm - 12 fatias).');

-- =====================================================
-- Create Indexes for Performance
-- =====================================================

CREATE INDEX IF NOT EXISTS idx_pizzas_category ON kernelmind.pizzas(category);
CREATE INDEX IF NOT EXISTS idx_pizzas_is_available ON kernelmind.pizzas(is_available);
CREATE INDEX IF NOT EXISTS idx_orders_customer_id ON kernelmind.orders(customer_id);
CREATE INDEX IF NOT EXISTS idx_orders_status ON kernelmind.orders(status);
CREATE INDEX IF NOT EXISTS idx_order_items_order_id ON kernelmind.order_items(order_id);
CREATE INDEX IF NOT EXISTS idx_chat_messages_session_id ON kernelmind.chat_messages(session_id);
CREATE INDEX IF NOT EXISTS idx_vector_documents_type ON kernelmind.vector_documents(document_type);

-- =====================================================
-- Grant Permissions
-- =====================================================

-- Grant usage on schema
GRANT USAGE ON SCHEMA kernelmind TO PUBLIC;

-- Grant permissions on tables
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA kernelmind TO PUBLIC;

-- Grant permissions on sequences
GRANT USAGE ON ALL SEQUENCES IN SCHEMA kernelmind TO PUBLIC;

-- =====================================================
-- Done!
-- =====================================================
